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

        public AddOrEditUserWindow(Client clientOrNull)
        {
            InitializeComponent();
             if (clientOrNull == null)
                Client = new Client();
            else
                Client = clientOrNull;
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
    }
}
