using denta_med_crm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace denta_med_crm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Database db = new Database();

        public MainWindow()
        {
            InitializeComponent();

            //db init

            InitializeOrUpdate();

            foreach (var column in dataGrid.Columns)
            {
                var item = new MenuItem();
                item.IsCheckable = true;
                item.IsChecked = true; //default
                item.Header = column.Header;
                item.Checked += (x,y) =>
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


        public void InitializeOrUpdate()
        {
            if (dataGrid.ItemsSource == null)
                dataGrid.ItemsSource = db.Clients;
            else dataGrid.Items.Refresh();
            
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

                        Tooth_11="x",
                        Tooth_12="x",
                        Tooth_13="x",
                        Tooth_14="x",
                        Tooth_15="x",
                        Tooth_16="x",
                        Tooth_17="x",
                        Tooth_18="x",

                        Tooth_21="x",
                        Tooth_22="x",
                        Tooth_23="x",
                        Tooth_24="x",
                        Tooth_25="x",
                        Tooth_26="x",
                        Tooth_27="x",
                        Tooth_28="x",

                        Tooth_31="x",
                        Tooth_32="x",
                        Tooth_33="x",
                        Tooth_34="x",
                        Tooth_35="x",
                        Tooth_36="x",
                        Tooth_37="x",
                        Tooth_38="x",

                        Tooth_41="x",
                        Tooth_42="x",
                        Tooth_43="x",
                        Tooth_44="x",
                        Tooth_45="x",
                        Tooth_46="x",
                        Tooth_47="x",
                        Tooth_48="x"
                    }, new Inspection(){
                        InspectionDate=DateTime.Now.AddDays(10),
                        Doctor="Николай",
                        Description="asdfasdfa",

                        Tooth_11="z",
                        Tooth_12="z",
                        Tooth_13="z",
                        Tooth_14="z",
                        Tooth_15="z",
                        Tooth_16="z",
                        Tooth_17="z",
                        Tooth_18="z",

                        Tooth_21="z",
                        Tooth_22="z",
                        Tooth_23="z",
                        Tooth_24="z",
                        Tooth_25="z",
                        Tooth_26="z",
                        Tooth_27="z",
                        Tooth_28="z",

                        Tooth_31="z",
                        Tooth_32="z",
                        Tooth_33="z",
                        Tooth_34="z",
                        Tooth_35="z",
                        Tooth_36="z",
                        Tooth_37="z",
                        Tooth_38="z",

                        Tooth_41="z",
                        Tooth_42="z",
                        Tooth_43="z",
                        Tooth_44="z",
                        Tooth_45="z",
                        Tooth_46="z",
                        Tooth_47="z",
                        Tooth_48="z"
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
            new AddOrEditUserWindow(client);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
