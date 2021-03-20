﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountBusinessService = net.atos.daf.ct2.accountservice;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class EnumValidator
    {
        public static bool ValidateImageType(char enumValue)
        {
            string enumList = "JPTGWSRB";
            if (enumList.Contains(enumValue))
            {
                return true;
            }
            return false;
        }
        public static bool ValidateAccountType(char accountType)
        {
            string enumList = "pPSs";
            if (enumList.Contains(accountType))
            {
                return true;
            }
            return false;
        }
        public static bool ValidateAccessType(char accountType)
        {
            string enumList = "vVfF";
            if (enumList.Contains(accountType))
            {
                return true;
            }
            return false;
        }
        public static bool ValidateGroupType(char groupType)
        {
            string enumList = "sSgGdD";
            if (enumList.Contains(groupType))
            {
                return true;
            }
            return false;
        }

        public static bool ValidateVehicleStatus(char statusType)
        {
            string enumList = "nNiIuUtToO";
            if (enumList.Contains(statusType))
            {
                return true;
            }
            return false;
        }
    }
}

