using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using DynamicData;
using Newtonsoft.Json;
using ReactiveUI;

namespace denta_med_crm.Model
{
    [DataContract]
    [Serializable]
    public class Client : ReactiveObject
    {
        public Client()
        {
            this.WhenAnyValue(x => x.Procedures, x => x.Inspections)
                .Where(x => x.Item1 != null && x.Item2 != null)
                .Subscribe(x =>
                {
                    var (procedures, inspections) = x;
                    procedures.CollectionChanged += Procedures_CollectionChanged;
                    inspections.CollectionChanged += Inspections_CollectionChanged;
                });
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

        [DataMember] [JsonProperty("id")] public string? Id { get; set; }
        [DataMember] [JsonProperty("full_name")] public string FullName { get; set; } = "";
        [DataMember] [JsonProperty("sex")] public Sex Sex { get; set; } = Sex.Male;

        [DataMember]
        [JsonProperty("client_description")]
        public string ClientDescription { get; set; } = "";

        //подсветить если сегодня его день рождения или в пределах 7 дней в карточке
        [DataMember]
        [JsonProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        //Типа он с нами c такого-то (сколько-то в карточке)
        [DataMember]
        [JsonProperty("first_visit")]
        public DateTime FirstVisit { get; set; } = DateTime.Now;

        //0 - нет, если есть то сумма скидки в % вводится
        [DataMember]
        [JsonProperty("discount")]
        public int Discount { get; set; }

        //основной
        [DataMember]
        [JsonProperty("main_phone_number")]
        public string MainPhoneNumber { get; set; } = "";

        //альтернативный
        [DataMember]
        [JsonProperty("alternative_phone_number")]
        public string AlternativePhoneNumber { get; set; } = "";

        //список процедур открывается в отдельном окне
        [DataMember]
        [JsonProperty("procedures")]
        public ObservableCollection<Procedure> Procedures { get; set; }

        [DataMember]
        [JsonProperty("inspections")]
        public ObservableCollection<Inspection> Inspections { get; set; }

        [IgnoreDataMember]
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

        [IgnoreDataMember]
        [JsonIgnore]
        public string LastVisitInfo
        {
            get
            {
                var max = FirstVisit.Ticks;
                var proceduresF = Procedures.Where(x => DateTime.Now > x.ProcedureDate);
                if (proceduresF.Count() != 0)
                    max = Math.Max(max, proceduresF.Max(x => x.ProcedureDate.Ticks));
                if (Inspections.Count != 0)
                    max = Math.Max(max, Inspections.Max(x => x.InspectionDate.Ticks));
                return new DateTime(max).ToString("dd.MM.yyyy H:mm");
            }
        }
    }
}