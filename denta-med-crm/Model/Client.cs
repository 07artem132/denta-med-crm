using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [Serializable]
    public class Client
    {
        [JsonProperty("first_name")]
        public string FullName;
        //подсветить если сегодня его день рождения или в пределах 7 дней в карточке
        [JsonProperty("date_of_birth")]
        public DateTime DateOfBirth;
        //Типа он с нами c такого-то (сколько-то в карточке)
        [JsonProperty("first_visit")]
        public DateTime FirstVisit;
        //Типа он был у нас последний раз ....
        [JsonProperty("last_visit")]
        public DateTime LastVisit;
        //0 - нет, если есть то сумма скидки в % вводится
        [JsonProperty("discount")]
        public int Discount;
        //основной
        [JsonProperty("main_phone_number")]
        public string MainPhoneNumber;
        //альтернативный
        [JsonProperty("alternative_phone_number")]
        public string AlternativePhoneNumber;
        //список процедур открывается в отдельном окне
        [JsonProperty("procedures")]
        public List<Procedure> Procedures;
    }
}
