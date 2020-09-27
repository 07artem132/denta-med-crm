using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [Serializable]
    public class Inspection
    {
        [JsonProperty("inspection_date")]
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        //Авто подсказка по первым буквам (поиск по всей строке прошлых докторов которые когда либо и кому либо были указаны, при поиске приведение к одному регистру)
        [JsonProperty("doctor")]
        public Doctor Doctor { get; set; } = new Doctor("");
        [JsonProperty("description")]
        public string Description { get; set; } = "";
        //Зубы
        [JsonProperty("teeth")]
        public string[] Teeth { get; set; } = new string[32];

        public string GetToothData(int num)
        {
            var x = num / 10;
            var y = num - x * 10;
            var index = y-1 + (x - 1) * 8;
            return Teeth[index];
        }

        public void SetToothData(int num, string data)
        {
            var x = num / 10;
            var y = num - x * 10;
            var index = y-1 + (x - 1) * 8;
            Teeth[index] = data;
        }
    }
}
