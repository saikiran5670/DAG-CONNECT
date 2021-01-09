using System;

namespace net.atos.daf.ct2.accountpreference
{
    public enum CurrencyType
    {
         None = 'N',
        Euro = 'E',
        US_Dollar= 'U',
        Pond_Sterlingr= 'P'
    }
    public enum VehicleDisplay
    {
        None = 'N',
        Registration_Number= 'R',
        Name= 'N',
        VIN = 'V'
    }

    public enum Unit
    {
        None = 'N',
        Metric= 'M',
        Imperial= 'I',
        US_Imperial = 'U'
    }
    public enum DateFormatDisplay
    {
        None = 'N',
        Day_Month_Year= 'D',
        Month_Day_Year= 'M',
        Year_Month_Day= 'Y',
    }
}
