using System.Collections.Generic;

namespace net.atos.daf.ct2.fms.entity
{
    public static class DriverWorkingState
    {
        public static Dictionary<int, string> driverWorkingState = new Dictionary<int, string>() {
            {0,"Rest"},
            {1,"Driver available"},
            {2,"Work"},
            {3,"Drive"},
            {4,""},
            {5,""},
            {6,"Error"},
            {7,"Not available"},
        };



        //0= Rest - sleeping
        //1= Driver available – short break
        //2= Work – loading, unloading, working in an office
        //3= Drive – behind wheel
        //4-5= Not Used - Value available to be defined
        //6= Error
        //7= Not available


    }
}
