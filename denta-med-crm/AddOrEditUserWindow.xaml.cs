using denta_med_crm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace denta_med_crm
{
    /// <summary>
    /// Логика взаимодействия для AddOrEditUserWindow.xaml
    /// </summary>
    public partial class AddOrEditUserWindow : Window
    {
        public Client Client
        {
            get { return (Client)GetValue(ClientProperty); }
            set { SetValue(ClientProperty, value); }
        }

        public static readonly DependencyProperty ClientProperty;
        public Client AddedUser;

        private int proceduresCurrentTeeth = -1;
        private bool disallowEditingTeeth;

        public AddOrEditUserWindow(Client clientOrNull, Procedure redirectTo = null)
        {
            InitializeComponent();



            _undoButton.Visibility = clientOrNull == null ? Visibility.Visible : Visibility.Hidden;
            if (clientOrNull == null)
                Client = new Client();
            else Client = clientOrNull;

            var enu = new Grid[] { _ts1, _ts2, _ts3, _ts4 };
            foreach (var grid in enu)
                foreach (var ui in grid.Children)
                    if (ui is TextBox tb)
                        tb.TextChanged += OnToothTextboxChanged;

            _inspections.SelectionChanged += (x, y) =>
            {
                var selectedInspection = _inspections.SelectedItem as Inspection;
                if (selectedInspection == null)
                    return;
                LoadToothData(selectedInspection);
            };

            var dt = new DispatcherTimer();
            dt.Tick += (x, y) =>
            {
                _daysToBirthday.Content = Client.DaysToBirthday;
                _lastVisitInfo.Content = Client.LastVisitInfo;
            };
            dt.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dt.Start();

            if (redirectTo != null)
            {
                _tab.SelectedIndex = 2;
                _procedures.SelectedItem = redirectTo;
            }

            /*
            _TEST.AutoSuggestionList = new List<string>()
            {
                "qwer",
                "erewt",
                "gerger",
                "erher",
                "erher1",
                "erher2",
                "erher3",
                "erher4",
                "erher5",
                "erher6",
            };*/
        }
        private void updateAvalibleTeethHistory()
        {
            HashSet<int> available = new HashSet<int>();
            foreach (var inspection in Client.Inspections)
            {
                for (int i = 1; i <= 8; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        var fullNm = j * 10 + i;
                        var result = inspection.GetToothData(fullNm);
                        if (!string.IsNullOrEmpty(result))
                            available.Add(fullNm);
                    }
                }
            }
            foreach (var procedure in Client.Procedures)
            {
                for (int i = 1; i <= 8; i++)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        var fullNm = j * 10 + i;
                        var result = procedure.GetToothData(fullNm);
                        if (!string.IsNullOrEmpty(result))
                            available.Add(fullNm);
                    }
                }
            }
            teethViewTest.SetAvailableTeeth(available.ToArray());
        }
        static AddOrEditUserWindow()
        {
            ClientProperty = DependencyProperty.Register("Client", typeof(Client), typeof(AddOrEditUserWindow));
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AddedUser = Client;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void LoadToothData(Inspection from)
        {
            disallowEditingTeeth = true;
            var enu = new Grid[] { _ts1, _ts2, _ts3, _ts4 };
            foreach (var grid in enu)
                foreach (var ui in grid.Children)
                {
                    if (ui is TextBox tb && tb.Tag != null)
                    {
                        tb.Text = from.GetToothData(int.Parse(tb.Tag.ToString()));
                    }
                }
            disallowEditingTeeth = false;
        }

        private void OnToothTextboxChanged(object sender, TextChangedEventArgs args)
        {
            if (disallowEditingTeeth)
                return;

            var textBox = ((TextBox)sender);
            var num = int.Parse(textBox.Tag.ToString());

            var selectedInspection = _inspections.SelectedItem as Inspection;
            if (selectedInspection == null)
                return;

            selectedInspection.SetToothData(num, textBox.Text);
        }

        private void _addFormula_Click(object sender, RoutedEventArgs e)
        {
            Client.Inspections.Add(new Inspection());
            _inspections.SelectedIndex = _inspections.Items.Count - 1;
        }

        private void _addLabel_Click(object sender, RoutedEventArgs e)
        {
            Client.Procedures.Add(new Procedure());
            _procedures.SelectedIndex = _procedures.Items.Count - 1;
        }

        private void Delete_inspection(object sender, RoutedEventArgs e)
        {
            var selectedItem = _inspections.SelectedItem;
            if (selectedItem is Inspection inspection)
            {
                Client.Inspections.Remove(inspection);
            }
        }

        private void Delete_record(object sender, RoutedEventArgs e)
        {
            var selectedItem = _procedures.SelectedItem;
            if (selectedItem is Procedure procedure)
            {
                Client.Procedures.Remove(procedure);
            }
        }

        private void MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }



        private void _qTest_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void _inspections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _inspectionsGrid.IsEnabled = _inspections.SelectedItem != null;
        }

        private void _procedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _proceduresGrid.IsEnabled = _procedures.SelectedItem != null;
        }

        private void _proceduresGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_proceduresGrid.DataContext == null)
                return;
            var newDC = (Procedure)_proceduresGrid.DataContext;

            List<int> available = new List<int>();
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    var fullNm = j * 10 + i;
                    if (!string.IsNullOrEmpty(newDC.GetToothData(fullNm)))
                        available.Add(fullNm);
                }
            }
            _teethViewProcedures.SetAvailableTeeth(available.ToArray());
        }

        private void _teethViewProcedures_ToothPicked(int obj)
        {
            var newDC = (Procedure)_proceduresGrid.DataContext;
            proceduresCurrentTeeth = obj;
            _teethProcedureDescription.Text = newDC.GetToothData(obj);
        }

        private void _teethProcedureDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (proceduresCurrentTeeth != -1)
            {
                var newDC = (Procedure)_proceduresGrid.DataContext;
                newDC.SetToothData(proceduresCurrentTeeth, _teethProcedureDescription.Text);
                _proceduresGrid_DataContextChanged(null, new DependencyPropertyChangedEventArgs());
            }
        }

        private void teethViewTest_ToothPicked(int obj)
        {
            _teethInfo.Items.Clear();
            foreach (var inspection in Client.Inspections)
            {
                var result = inspection.GetToothData(obj);
                if (!string.IsNullOrEmpty(result))
                    _teethInfo.Items.Add(new string[4] {
                        inspection.InspectionDate.ToString("dd.MM.yyyy"),
                        "Осмотр",
                        inspection.Doctor,
                        _inspection_reduction_decipher(result)
                    });
            }
            foreach (var procedure in Client.Procedures)
            {
                var result = procedure.GetToothData(obj);
                if (!string.IsNullOrEmpty(result))
                    _teethInfo.Items.Add(new string[4] {
                        procedure.ProcedureDate.ToString("dd.MM.yyyy"),
                        "Процедура",
                        procedure.Doctor,
                        result
                    });
            }
            _teethInfo.Items.Refresh();
        }

        private string _inspection_reduction_decipher(string reduction)
        {
            return reduction;

            // case :
            //   return "Отсутствует";
            //case "R":
            //    return "Корень";
            //case "P":
            //    return "Пульпит";
            //case "Pt":
            //    return "Переодонтит";
            //case "A":
            //    return "Пародонтоз";
            //case "К":
            //    return "Коронка";
            //case "C":
            //    return "Кариес";
            //case "П":
            //    return "Пломбированный";
            //case "И":
            //    return "Искусственный зуб";
            //case "П/С":
            //    return "Пломба/Кариес";


        }

        private void _tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tab.SelectedIndex == 3)//your specific tabname
            {
                updateAvalibleTeethHistory();
            }

        }
    }
}
