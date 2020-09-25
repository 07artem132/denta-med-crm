using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [Serializable]
    public class Procedure
    {
        //Авто подсказка по первым буквам (поиск по всей строке любых процедур которые когда либо и кому либо были проведены, при поиске приведение к одному регистру)
        [JsonProperty("procedure_name")]
        public string ProcedureName { get; set; }
        [JsonProperty("procedure_duration")]
        public int ProcedureDuration { get; set; }
        //до какого гарантия, автоматически выставлять +1 год от текущей даты
        [JsonProperty("warranty_period")]
        public DateTime WarrantyPeriod { get; set; }
        //по умолчанию текущая дата может быть будующей датой (записан пациент на какое-то число)
        [JsonProperty("procedure_date")]
        public DateTime ProcedureDate { get; set; }
        //Выполнена или нет
        [JsonProperty("completed")]
        public bool Сompleted { get; set; }
        //Авто подсказка по первым буквам (поиск по всей строке прошлых докторов которые когда либо и кому либо были указаны, при поиске приведение к одному регистру)
        [JsonProperty("doctor")]
        public string Doctor { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
