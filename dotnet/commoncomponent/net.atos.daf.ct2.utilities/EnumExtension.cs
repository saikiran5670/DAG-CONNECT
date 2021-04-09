﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static net.atos.daf.ct2.utilities.CommonEnums;

namespace net.atos.daf.ct2.utilities
{
    public static class EnumExtension
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static char GetState(bool state)
        {
            return state ? (char)State.Active : (char)State.Inactive;
        }
    }
}
