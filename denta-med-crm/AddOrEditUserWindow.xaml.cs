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
        public Client EditableClient;

        public AddOrEditUserWindow()
        {
            InitializeComponent();
        }


        public void Init(Client clientOrNull)
        {
            this.EditableClient = clientOrNull;

            //
        }
    }
}
