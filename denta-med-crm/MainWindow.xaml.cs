using denta_med_crm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

namespace denta_med_crm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string dbFile = "db.json";
        Database db = new Database(dbFile);

        public MainWindow()
        {
            InitializeComponent();
            Style s = new Style();
            s.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));
            _tabControl.ItemContainerStyle = s;

            if (File.Exists(dbFile))
                db.Import(dbFile);

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
            InitializeOrUpdate(x => x.FullName.Contains(this._filterINput.Text));
        }
        private void patient_shuduler_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tabControl.SelectedIndex != 1)
                return;

            _patient_shuduler.SelectedDate = DateTime.Now;
            _patient_shuduler.Mode = Mode.Day;
            _patient_shuduler.Events.Clear();
            foreach (Client Client in db.Clients)
                foreach (Procedure Procedure in Client.Procedures)
                    _patient_shuduler.AddEvent(
                       new Event()
                       {
                           Subject = string.Format("Доктор: {0}\rПроцедура: {1}", Procedure.Doctor, Procedure.Description),
                           Color = Brushes.LightGreen,
                           Start = Procedure.ProcedureDate,
                           End = Procedure.ProcedureDate.AddMinutes(Procedure.ProcedureDuration),
                           RelObject = new object[] { Client, Procedure },
                       });

        }
        void patient_shuduler_OnScheduleDoubleClick(object sender, DateTime e)
        {
        }
        void patient_shuduler_OnEventDoubleClick(object sender, Event e)
        {
            new AddOrEditUserWindow((Client)e.RelObject[0], (Procedure)e.RelObject[1]).Show();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client)dataGrid.SelectedItem;
            new AddOrEditUserWindow(client).ShowDialog();
            dataGrid.Items.Refresh();
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
                db.Import(dlg.FileName);
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
    }


}
