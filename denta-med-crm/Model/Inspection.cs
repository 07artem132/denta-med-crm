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
        public DateTime InspectionDate { get; set; }
        //Авто подсказка по первым буквам (поиск по всей строке прошлых докторов которые когда либо и кому либо были указаны, при поиске приведение к одному регистру)
        [JsonProperty("doctor")]
        public string Doctor { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }


        [JsonProperty("teeth")]
        public string[] Teeth;

        public string GetToothData(int num)
        {
            var x = num / 10;
            var y = num - x;
            var index = num + x * 8;
            return Teeth[index];
        }

        public void SetToothData(int num, string data)
        {
            var x = num / 10;
            var y = num - x;
            var index = num + x * 8;
            Teeth[index] = data;
        }

        /*#region зубы   
        [JsonProperty("tooth_18")]
        public string Tooth_18 { get; set; }
        [JsonProperty("tooth_17")]
        public string Tooth_17 { get; set; }
        [JsonProperty("tooth_16")]
        public string Tooth_16 { get; set; }
        [JsonProperty("tooth_15")]
        public string Tooth_15 { get; set; }
        [JsonProperty("tooth_14")]
        public string Tooth_14 { get; set; }
        [JsonProperty("tooth_13")]
        public string Tooth_13 { get; set; }
        [JsonProperty("tooth_12")]
        public string Tooth_12 { get; set; }
        [JsonProperty("tooth_11")]
        public string Tooth_11 { get; set; }

        [JsonProperty("tooth_28")]
        public string Tooth_28 { get; set; }
        [JsonProperty("tooth_27")]
        public string Tooth_27 { get; set; }
        [JsonProperty("tooth_26")]
        public string Tooth_26 { get; set; }
        [JsonProperty("tooth_25")]
        public string Tooth_25 { get; set; }
        [JsonProperty("tooth_24")]
        public string Tooth_24 { get; set; }
        [JsonProperty("tooth_23")]
        public string Tooth_23 { get; set; }
        [JsonProperty("tooth_22")]
        public string Tooth_22 { get; set; }
        [JsonProperty("tooth_21")]
        public string Tooth_21 { get; set; }

        [JsonProperty("tooth_38")]
        public string Tooth_38 { get; set; }
        [JsonProperty("tooth_37")]
        public string Tooth_37 { get; set; }
        [JsonProperty("tooth_36")]
        public string Tooth_36 { get; set; }
        [JsonProperty("tooth_35")]
        public string Tooth_35 { get; set; }
        [JsonProperty("tooth_34")]
        public string Tooth_34 { get; set; }
        [JsonProperty("tooth_33")]
        public string Tooth_33 { get; set; }
        [JsonProperty("tooth_32")]
        public string Tooth_32 { get; set; }
        [JsonProperty("tooth_31")]
        public string Tooth_31 { get; set; }  
        
        [JsonProperty("tooth_48")]
        public string Tooth_48 { get; set; }
        [JsonProperty("tooth_47")]
        public string Tooth_47 { get; set; }
        [JsonProperty("tooth_46")]
        public string Tooth_46 { get; set; }
        [JsonProperty("tooth_45")]
        public string Tooth_45 { get; set; }
        [JsonProperty("tooth_44")]
        public string Tooth_44 { get; set; }
    [JsonProperty("tooth_43")]
        public string Tooth_43 { get; set; }
    [JsonProperty("tooth_42")]
        public string Tooth_42 { get; set; }
    [JsonProperty("tooth_41")]
        public string Tooth_41 { get; set; }
    #endregion*/
}
}
