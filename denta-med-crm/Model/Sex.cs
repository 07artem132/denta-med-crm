using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denta_med_crm.Model
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Sex
    {
        [Description("Мужчина")]
        Male,
        [Description("Женщина")]
        Female
    }
}
