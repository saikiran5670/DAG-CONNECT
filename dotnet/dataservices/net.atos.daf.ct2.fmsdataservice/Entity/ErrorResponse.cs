﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.fmsdataservice.Entity
{
    public class ErrorResponse
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public string Value { get; set; }

    }
}
