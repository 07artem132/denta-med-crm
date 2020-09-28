using denta_med_crm.Model;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfScheduler;
using Path = System.IO.Path;

namespace denta_med_crm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string dbFile = "db.json";
        internal static Database db = new Database(dbFile);

        public MainWindow()
        {
            InitializeComponent();
            Style s = new Style();
            s.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));
            _tabControl.ItemContainerStyle = s;
            
            InitializeOrUpdate();

            foreach (var column in dataGrid.Columns)
            {
                var item = new MenuItem();
                item.IsCheckable = true;
                item.IsChecked = true; //default
                item.Header = column.Header;
                item.Checked += (x, y) =>
                {
                    column.Visibility = Visibility.Visible;
                };
                item.Unchecked += (x, y) =>
                {
                    column.Visibility = Visibility.Hidden;
                };
                menuFields.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            db.Export(dbFile);
            db.Dispose();
        }

        public void InitializeOrUpdate(Func<Client, bool> filter = null)
        {
            if (dataGrid.ItemsSource == null)
            {
                if (filter != null)
                    dataGrid.ItemsSource = db.Clients.Where(filter);
                else dataGrid.ItemsSource = db.Clients;
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
            var context = ((System.Windows.Controls.Button)e.Source).DataContext as Client;

            //TODO: Code
        }

        private void BtAddUser_Click(object sender, RoutedEventArgs e)
        {
            var temp = new AddOrEditUserWindow(null);
            temp.ShowDialog();
            if (temp.AddedUser != null)
            {
                db.Clients.Add(temp.AddedUser);
                db.Export(dbFile);
                dataGrid.Items.Refresh();
            }
        }

        private void BtAddEvent_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client)dataGrid.SelectedItem;

        }//!!!!

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeOrUpdate(x =>
            {
                if (x.FullName.ToUpper().Contains(this._filterINput.Text.ToUpper()))
                    return true;
                else if (x.MainPhoneNumber.ToUpper().Contains(this._filterINput.Text.ToUpper()))
                    return true;
                else if (x.AlternativePhoneNumber.ToUpper().Contains(this._filterINput.Text.ToUpper()))
                    return true;
                else return false;  
            });
        }
        private void patient_shuduler_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tabControl.SelectedIndex != 1)
                return;

            _patient_shuduler.SelectedDate = DateTime.Now;
            _patient_shuduler.Mode = Mode.Week;
            _patient_shuduler.Events.Clear();
            foreach (Client Client in db.Clients)
                AddEvents(Client);

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
                       new Event()
                       {
                           Subject = string.Format("Доктор: {0}\r", procedure.Doctor),
                           Color = Brushes.LightGreen,
                           Start = procedure.ProcedureDate,
                           End = procedure.ProcedureDate.AddMinutes(procedure.ProcedureDuration),
                           RelObject = new object[] { client, procedure },
                       });
        }
        void patient_shuduler_OnScheduleDoubleClick(object sender, DateTime e)
        {
        }
        void patient_shuduler_OnEventDoubleClick(object sender, Event e)
        {
            RemoveEvents((Client)e.RelObject[0]);
            new AddOrEditUserWindow((Client)e.RelObject[0], (Procedure)e.RelObject[1]).ShowDialog();
            dataGrid.Items.Refresh();
            AddEvents((Client)e.RelObject[0]);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client)dataGrid.SelectedItem;
            RemoveEvents(client);
            new AddOrEditUserWindow(client).ShowDialog();
            dataGrid.Items.Refresh();
            AddEvents(client);
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
                db.Export(dbFile);
                dataGrid.Items.Refresh();
            }
        }

        private void DataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void MenuItem_ExportDb(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Сохранить базу данных";
            dlg.Filter = $"DB Files (*.json)|*.json";
            var result = dlg.ShowDialog();
            if (result == true)
                db.Export(dlg.FileName);
        }

        private void MenuItem_ImportDb(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Открыть существующую базу данных?";
            dlg.Multiselect = false;
            dlg.Filter = $"DB Files (*.json)|*.json";
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
                using (TaskService ts = new TaskService())
                {
                    var result = ts.FindTask(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                    if (result != null)
                    {
                        _autorunItem.IsChecked = true;
                        var actions = result.Definition.Actions;
                        var action = (ExecAction)actions[0];
                        if (action.Path == Assembly.GetEntryAssembly().Location)
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
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        private void createTask()
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Авто запуск CRM";

                var dt = new LogonTrigger();
                dt.UserId = Environment.UserName;
                td.Triggers.Add(dt);
                td.Actions.Add(new ExecAction(System.Reflection.Assembly.GetEntryAssembly().Location, null, AssemblyDirectory));
                ts.RootFolder.RegisterTaskDefinition(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, td);
            }
        }
        private void deleteTask()
        {
            using (TaskService ts = new TaskService())
            {
                var result = ts.FindTask(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                if (result != null)
                {
                    ts.RootFolder.DeleteTask(result.Name);
                }
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
