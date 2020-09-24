using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [Serializable]
    public class Client
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("sex")]
        public Sex Sex { get; set; }
        [JsonProperty("client_description")]
        public string ClientDescription { get; set; }
        //подсветить если сегодня его день рождения или в пределах 7 дней в карточке
        [JsonProperty("date_of_birth")]
        public DateTime DateOfBirth { get; set; }
        //Типа он с нами c такого-то (сколько-то в карточке)
        [JsonProperty("first_visit")]
        public DateTime FirstVisit { get; set; }
        //Типа он был у нас последний раз ....
        [JsonProperty("last_visit")]
        public DateTime LastVisit { get; set; }

        [JsonIgnore]
        public string ClientLong
        {
            get
            {
                double diffSec = (DateTime.Now -FirstVisit  ).TotalSeconds;
                if (diffSec < 30 * 86400)
                {
                    return String.Format("{0} дня(-ей)", Math.Ceiling(diffSec / 86400));
                } else
                {
                    return String.Format("{0} дня(-ей)", Math.Ceiling(diffSec / 2628000));

                }
            }
        }
        //0 - нет, если есть то сумма скидки в % вводится
        [JsonProperty("discount")]
        public int Discount { get; set; }
        //основной
        [JsonProperty("main_phone_number")]
        public string MainPhoneNumber { get; set; }
        //альтернативный
        [JsonProperty("alternative_phone_number")]
        public string AlternativePhoneNumber { get; set; }
        //список процедур открывается в отдельном окне
        [JsonProperty("procedures")]
        public ObservableCollection<Procedure> Procedures { get; set; }
        [JsonProperty("inspections")]
        public ObservableCollection<Inspection> Inspections { get; set; }
    }
}
