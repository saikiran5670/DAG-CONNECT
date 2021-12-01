using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.schedular.Enum
{
    public enum DataPurgeEnum
    {
        Ok = 'O',
        Oknoop = 'N',
        Connectionfailed = 'C',
        Sqlerror = 'S',
        Timeout = 'T'

    }
}


//"O" - "enumdatapurging_Ok"
//"N" - "enumdatapurging_Oknoop"
//"C" - "enumdatapurging_Connectionfailed"
//"S" - "enumdatapurging_Sqlerror"
//"T" - "enumdatapurging_Timeout"

//SELECT id, type, enum, parent_enum, key, feature_id FROM translation.enumtranslation where type= 'G';




