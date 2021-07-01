
namespace net.atos.daf.ct2.reports.ENUM
{
    public enum CategoryState
    {
        Logisticalerts = 'L',
        Fuelanddriverperformance = 'F',
        Repairandmaintenance = 'R'
    }

    public enum AlertState
    {
        Critical = 'C',
        Warning = 'W',
        Advisory = 'A'
    }
    public enum HealthState
    {
        Noaction = 'N',
        Servicenow = 'V',
        Stopnow = 'T'
    }
    public enum OtherState
    {
    Nevermoved = 'N'
    }
}
