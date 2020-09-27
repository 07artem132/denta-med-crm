using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [Serializable]
    public class Client
    {
        public Client()
        {
            Procedures.CollectionChanged += Procedures_CollectionChanged;
            Inspections.CollectionChanged += Inspections_CollectionChanged;
        }

        private void Inspections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Inspection item in e.OldItems)
                {
                    MainWindow.db?.OnInspectionRemoved(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Inspection item in e.NewItems)
                {
                    MainWindow.db?.OnInspectionAdded(item);
                }
            }
        }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Procedure item in e.OldItems)
                {
                    MainWindow.db?.OnProcedureRemoved(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Procedure item in e.NewItems)
                {
                    MainWindow.db?.OnProcedureAdded(item);
                }
            }
        }

        [JsonProperty("full_name")]
        public string FullName { get; set; } = "";
        [JsonProperty("sex")]
        public Sex Sex { get; set; } = Sex.Male;
        [JsonProperty("client_description")]
        public string ClientDescription { get; set; } = "";
        //подсветить если сегодня его день рождения или в пределах 7 дней в карточке
        [JsonProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        //Типа он с нами c такого-то (сколько-то в карточке)
        [JsonProperty("first_visit")]
        public DateTime FirstVisit { get; set; } = DateTime.Now;
        //0 - нет, если есть то сумма скидки в % вводится
        [JsonProperty("discount")]
        public int Discount { get; set; } = 0;
        //основной
        [JsonProperty("main_phone_number")]
        public string MainPhoneNumber { get; set; } = "";
        //альтернативный
        [JsonProperty("alternative_phone_number")]
        public string AlternativePhoneNumber { get; set; } = "";
        //список процедур открывается в отдельном окне
        [JsonProperty("procedures")]
        public ObservableCollection<Procedure> Procedures { get; set; } = new ObservableCollection<Procedure>();
        [JsonProperty("inspections")]
        public ObservableCollection<Inspection> Inspections { get; set; } = new ObservableCollection<Inspection>();
        [JsonIgnore]
        public string DaysToBirthday
        {
            get
            {
                DateTime today = DateTime.Today;
                DateTime next = DateOfBirth.AddYears(today.Year - DateOfBirth.Year);

                if (next < today)
                    next = next.AddYears(1);

                int numDays = (next - today).Days;
                return string.Format("Через {0} дня(ей)", numDays);
            }
        }

        [JsonIgnore]
        public string LastVisitInfo
        {
            get
            {
                var max = FirstVisit.Ticks;
                var proceduresF = Procedures.Where(x => x.Сompleted);
                if (proceduresF.Count() != 0)
                    max = Math.Max(max, proceduresF.Max(x => x.ProcedureDate.Ticks));
                if (Inspections.Count != 0)
                    max = Math.Max(max, Inspections.Max(x => x.InspectionDate.Ticks));
                return new DateTime(max).ToString("dd.MM.yyyy H:mm");
            }
        }
    }
}
