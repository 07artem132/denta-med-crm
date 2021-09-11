using System.ComponentModel;

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
