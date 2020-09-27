using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace denta_med_crm.Model
{
    public class Doctor
    {
        public Doctor(string text)
        {
            Name=text;
        }
        public Doctor()
        {
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; MainWindow.db?.OnDoctorRename(); }
        }
    }
}
