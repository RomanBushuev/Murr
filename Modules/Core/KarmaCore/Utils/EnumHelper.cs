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

        public static T ToEnum<T>(this string value) where T:struct
        {
            var names = Enum.GetNames(typeof(T));
            foreach(var x in names)
            {
                Enum en = (Enum)Enum.Parse(typeof(T), x, true);
                if (value == en.ToDbAttribute())
                {
                    return (T)Enum.Parse(typeof(T), x, true); ;
                }
            }
            throw new ArgumentException("Not found");
        }

        public static string ToMoexFinAttribute(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (MoexFinAttribute[])fi.GetCustomAttributes(typeof(MoexFinAttribute), false);
            return attributes.Length > 0 ? attributes[0].Ident : null;
        }
    }
}
