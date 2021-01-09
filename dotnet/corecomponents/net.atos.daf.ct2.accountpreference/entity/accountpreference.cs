using System;

namespace net.atos.daf.ct2.accountpreference
{
    public class Accountpreference
    {
        public int Id { get; set; }
        public int Ref_Id { get; set; }
        public PreferenceType Type { get; set; }
        public int Language_Id { get; set; }
        public int Timezone_Id { get; set; }
        public CurrencyType Currency_Type { get; set; }
        public UnitType Unit_Type { get; set; }
        public VehicleDisplay VehicleDisplay_Type { get; set; }
        public DateFormatDisplay DateFormat_Type { get; set; }
        public bool Is_Active { get; set; }
       
    }
}
