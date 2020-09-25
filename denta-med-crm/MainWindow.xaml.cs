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
        Database db = new Database();
        const string dbFile = "db.json";

        public MainWindow()
        {
            InitializeComponent();
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


            var temp = new AddOrEditUserWindow(new Client()
            {
                FullName = "asdfasdf",
                MainPhoneNumber = "asd",
                AlternativePhoneNumber = "asdfasdf",
                Sex = Sex.Male,
                Discount = 10,
                FirstVisit = DateTime.Now.AddDays(-10),
                DateOfBirth = DateTime.Now.AddDays(5),
                LastVisit = DateTime.Now,
                ClientDescription = "asdfasdf",
                Inspections = new ObservableCollection<Inspection>() {
                    new Inspection(){
                        InspectionDate=DateTime.Now,
                        Doctor="Пупкин",
                        Description="asdfasdfa",


                    }, new Inspection(){
                        InspectionDate=DateTime.Now.AddDays(10),
                        Doctor="Николай",
                        Description="asdfasdfa",


                    }
                },
                Procedures = new ObservableCollection<Procedure>()
                {
                    new Procedure()
                    {
                        ProcedureName="Тестовая один",
                        ProcedureDuration=60,
                        WarrantyPeriod=DateTime.Now.AddDays(100),
                        ProcedureDate=DateTime.Now,
                        Сompleted=false,
                        Doctor="test1",
                    }, new Procedure()
                    {
                        ProcedureName="Тестовая два",
                        ProcedureDuration=11,
                        WarrantyPeriod=DateTime.Now.AddDays(99),
                        ProcedureDate=DateTime.Now.AddDays(-1),
                        Сompleted=true,
                        Doctor="test2",
                    }
                }
            });
            temp.Show();
            temp.Notify += x =>
            {
                db.Clients.Add(x);
                db.Export(dbFile);
            };
            dataGrid.Items.Refresh();
        }

        private void BtAddEvent_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client)dataGrid.SelectedItem;

        }

        private void BtEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
                return;
            var client = (Client)dataGrid.SelectedItem;
            new AddOrEditUserWindow(client).Show();
            dataGrid.Items.Refresh();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitializeOrUpdate(x => x.FullName.Contains(this._filterINput.Text));
        }
        private void patient_shuduler_Loaded(object sender, RoutedEventArgs e)
        {
            _patient_shuduler.SelectedDate = DateTime.Now;
            _patient_shuduler.Mode = Mode.Day;
            _patient_shuduler.Events.Clear();
            foreach (Client Client in db.Clients)
                foreach (Procedure Procedure in Client.Procedures)
                    _patient_shuduler.AddEvent(
                       new Event()
                       {
                           Subject = string.Format("Доктор: {0}\rПроцедура: {1}\rЗубов: {2}", Procedure.Doctor, Procedure.ProcedureName, Procedure.Tooth),
                           Color = Brushes.LightGreen,
                           Start = DateTime.Now,
                           End = DateTime.Now.AddMinutes(90),
                           RelObject = Client
                       });


        }
        void patient_shuduler_OnScheduleDoubleClick(object sender, DateTime e)
        {
        }
        void patient_shuduler_OnEventDoubleClick(object sender, Event e)
        {
            new AddOrEditUserWindow((Client)e.RelObject).Show();
        }
    }


}
