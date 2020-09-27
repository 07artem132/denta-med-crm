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
            dt.Tick += (x,y) =>
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
            _inspections.IsEnabled = _inspections.SelectedItem != null;
        }

        private void _procedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _proceduresGrid.IsEnabled = _procedures.SelectedItem != null;
        }
    }
}
