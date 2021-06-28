﻿using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public class UserTimeZone
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDateFormat : UserTimeZone
    {
    }

    public class UserTimeFormat : UserTimeZone
    {
        public string Key { get; set; }
    }
}
