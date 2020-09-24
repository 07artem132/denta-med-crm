using denta_med_crm.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

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

        public delegate void UpdateClient(Client client);
        public event UpdateClient Notify;

        private bool disallowEditingTeeth;
        
        public AddOrEditUserWindow(Client clientOrNull)
        {
            InitializeComponent();
             if (clientOrNull == null)
                Client = new Client();
            else
                Client = clientOrNull;

            _inspections.SelectionChanged += (x, y) =>
            {
                var selectedInspection = _inspections.SelectedItem as Inspection;
                if (selectedInspection == null)
                    return;
                LoadToothData(selectedInspection);
            };
        }
        static AddOrEditUserWindow()
        {
            ClientProperty = DependencyProperty.Register("Client", typeof(Client), typeof(AddOrEditUserWindow));
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Notify?.Invoke(Client);
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
                        tb.Text = from.GetToothData((int)tb.Tag);
                    }
                }
            disallowEditingTeeth = false;
        }

        private void OnToothTextboxChanged(object sender, TextChangedEventArgs args)
        {
            if (disallowEditingTeeth)
                return;

            var textBox = ((TextBox)sender);
            var num = (int)textBox.Tag;

            var selectedInspection = _inspections.SelectedItem as Inspection;
            if (selectedInspection == null)
                return;

            selectedInspection.SetToothData(num, textBox.Text);
        }
    }
}
