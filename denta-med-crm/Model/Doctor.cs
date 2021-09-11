namespace denta_med_crm.Model
{
    public struct Doctor
    {
        public Doctor(string text)
        {
            _name = text;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set {
                MainWindow.db?.OnDoctorRename(_name, value);
                _name = value;
                 }
        }
    }
}
