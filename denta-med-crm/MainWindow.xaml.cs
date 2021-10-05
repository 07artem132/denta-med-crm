using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Akavache;
using denta_med_crm.Model;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using WpfScheduler;

namespace denta_med_crm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Database db = new();

        public MainWindow()
        {
            InitializeComponent();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            var s = new Style();
            s.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));
            _tabControl.ItemContainerStyle = s;

            InitializeOrUpdate();
            
            foreach (var column in dataGrid.Columns)
            {
                var item = new MenuItem {IsCheckable = true, IsChecked = true, Header = column.Header};
                //default
                item.Checked += (x, y) => { column.Visibility = Visibility.Visible; };
                item.Unchecked += (x, y) => { column.Visibility = Visibility.Hidden; };
                menuFields.Items.Add(item);
            }
        }
       
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BlobCache.Shutdown().Wait();
            db.Dispose();
        }

        private void InitializeOrUpdate(Func<Client, bool> filter = null)
        {
            if (dataGrid.ItemsSource == null)
            {
                dataGrid.ItemsSource = filter != null ? db.Clients.Where(filter) : db.Clients;
            }
            else
            {
                if (filter != null)
                {
                    dataGrid.ItemsSource = db.Clients.Where(filter);
                    dataGrid.Items.Refresh();
                }
                else dataGrid.Items.Refresh();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var context = ((Button) e.Source).DataContext as Client;

            //TODO: Code
        }

        private void BtAddUser_Click(object sender, RoutedEventArgs e)
        {
            var temp = new AddOrEditUserWindow(null);
            temp.ShowDialog();
            if (temp.AddedUser != null)
            {
                var id = 0;
                if (db.Clients.Count > 0)
                {
                    id = db.Clients.Max(x => int.Parse(x.Id));
                }

                temp.AddedUser.Id = (++id).ToString();
                
                db.Clients.Add(temp.AddedUser);
                BlobCache.UserAccount.InsertObject(temp.AddedUser.Id, temp.AddedUser);
                BlobCache.UserAccount.Flush();
                dataGrid.Items.Refresh();
            }
        }

        private void BtAddEvent_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client) dataGrid.SelectedItem;
        } //!!!!

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeOrUpdate(x =>
            {
                if (x.FullName.ToUpper().Contains(_filterINput.Text.ToUpper()))
                    return true;
                if (x.MainPhoneNumber.ToUpper().Contains(_filterINput.Text.ToUpper()))
                    return true;
                if (x.AlternativePhoneNumber.ToUpper().Contains(_filterINput.Text.ToUpper()))
                    return true;
                return false;
            });
        }

        private void patient_shuduler_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tabControl.SelectedIndex != 1)
                return;

            _patient_shuduler.SelectedDate = DateTime.Now;
            _patient_shuduler.Mode = Mode.Week;
            _patient_shuduler.Events.Clear();
            foreach (var client in db.Clients)
                AddEvents(client);
        }

        private void RemoveEvents(Client client)
        {
            foreach (var ev in client.Procedures)
            {
                var result = _patient_shuduler.Events.FirstOrDefault(x => x.RelObject[1] == ev);
                if (result != null)
                    _patient_shuduler.Events.Remove(result);
            }
        }

        private void AddEvents(Client client)
        {
            foreach (var procedure in client.Procedures)
                _patient_shuduler.AddEvent(
                    new Event
                    {
                        Subject = $"Доктор: {procedure.Doctor}\r",
                        Color = Brushes.LightGreen,
                        Start = procedure.ProcedureDate,
                        End = procedure.ProcedureDate.AddMinutes(procedure.ProcedureDuration),
                        RelObject = new object[] {client, procedure},
                    });
        }

        void patient_shuduler_OnScheduleDoubleClick(object sender, DateTime e)
        {
        }

        void patient_shuduler_OnEventDoubleClick(object sender, Event e)
        {
            RemoveEvents((Client) e.RelObject[0]);
            new AddOrEditUserWindow((Client) e.RelObject[0], (Procedure) e.RelObject[1]).ShowDialog();
            dataGrid.Items.Refresh();
            AddEvents((Client) e.RelObject[0]);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client) dataGrid.SelectedItem;
            RemoveEvents(client);
            new AddOrEditUserWindow(client).ShowDialog();
            dataGrid.Items.Refresh();
            AddEvents(client);
            BlobCache.UserAccount.InsertObject(client.Id, client);
            BlobCache.UserAccount.Flush();
        }

        private void MenuItemTabClient_Click(object sender, RoutedEventArgs e)
        {
            _tabControl.SelectedIndex = 0;
        }

        private void MenuItemTabShudler_Click(object sender, RoutedEventArgs e)
        {
            _tabControl.SelectedIndex = 1;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dataGrid.SelectedItem;
            if (selectedItem is Client client)
            {
                db.Clients.Remove(client);
                BlobCache.UserAccount.Invalidate(client.Id);
                BlobCache.UserAccount.Flush();
                dataGrid.Items.Refresh();
            }
        }

        private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        }

        private void MenuItem_ExportDb(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Сохранить базу данных", Filter = "DB Files (*.json)|*.json"
            };
            var result = dlg.ShowDialog();
            if (result == true)
                db.Export(dlg.FileName);
        }

        private void MenuItem_ImportDb(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Открыть существующую базу данных?",
                Multiselect = false,
                Filter = "DB Files (*.json)|*.json"
            };
            var result = dlg.ShowDialog();
            if (result == true)
            {
                db.Import(dlg.FileName);
                dataGrid.ItemsSource = null;
                InitializeOrUpdate();
            }
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _sModeDay_Click(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.Mode = Mode.Day;
        }

        private void _sModeWeek_Click(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.Mode = Mode.Week;
        }

        private void _sModeMonth_Click(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.Mode = Mode.Month;
        }

        private void _sPrev_Click(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.PrevPage();
        }

        private void _sNext_Click(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.NextPage();
        }

        private void MenuItem_Initialized(object sender, EventArgs e)
        {
            try
            {
                using var ts = new TaskService();
                var result = ts.FindTask(Assembly.GetExecutingAssembly().GetName().Name);
                if (result == null) return;
                _autorunItem.IsChecked = true;
                var actions = result.Definition.Actions;
                var action = (ExecAction) actions[0];
                if (action.Path == Assembly.GetEntryAssembly()?.Location)
                    if (action.WorkingDirectory == AssemblyDirectory)
                    {
                        _autorunItem.IsChecked = true;
                    }
                    else
                    {
                        ts.RootFolder.DeleteTask(result.Name);
                        createTask();
                    }
                else
                {
                    ts.RootFolder.DeleteTask(result.Name);
                    createTask();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void createTask()
        {
            using var ts = new TaskService();
            var td = ts.NewTask();
            td.RegistrationInfo.Description = "Авто запуск CRM";

            var dt = new LogonTrigger {UserId = Environment.UserName};
            td.Triggers.Add(dt);
            td.Actions.Add(new ExecAction(Assembly.GetEntryAssembly().Location, null, AssemblyDirectory));
            ts.RootFolder.RegisterTaskDefinition(Assembly.GetExecutingAssembly().GetName().Name, td);
        }

        private void deleteTask()
        {
            using var ts = new TaskService();
            var result = ts.FindTask(Assembly.GetExecutingAssembly().GetName().Name);
            if (result != null)
            {
                ts.RootFolder.DeleteTask(result.Name);
            }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            createTask();
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            deleteTask();
        }
    }
}