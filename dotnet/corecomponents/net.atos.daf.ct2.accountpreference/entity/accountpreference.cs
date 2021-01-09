using System;

namespace net.atos.daf.ct2.accountpreference.entity
{
    public class accountpreference
    {
        public int Id { get; set; }
        public int RefId { get; set; }
        public int LanguageId { get; set; }
        public int TimezoneId { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public UnitType UnitType { get; set; }
        public VehicleDisplay VehicleDisplay { get; set; }
        public DateFormatDisplay DateFormatDisplay { get; set; }
        public bool IsActive { get; set; }
       
    }
}
