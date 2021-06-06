using System;
using System.ComponentModel;
using System.Reflection;

namespace KarmaCore.Utils
{
    public static class EnumHelper
    {
        public static string ToDbAttribute(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (MurrDbAttribute[])fi.GetCustomAttributes(typeof(MurrDbAttribute), false);
            return attributes.Length > 0 ? attributes[0].Ident : null;
        }

        public static string ToMoexFinAttribute(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (MoexFinAttribute[])fi.GetCustomAttributes(typeof(MoexFinAttribute), false);
            return attributes.Length > 0 ? attributes[0].Ident : null;
        }
    }
}
