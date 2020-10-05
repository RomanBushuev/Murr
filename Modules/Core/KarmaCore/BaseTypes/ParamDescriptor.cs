using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class ParamDescriptor
    {
        public ParamDescriptor()
        {

        }

        private string _ident;
        private string _description;
        private ParamType _paramType;
        private object _value;

        public string Ident { get { return _ident; } set { _ident = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public ParamType ParamType { get { return _paramType; }  set { _paramType = value; } }
        public object Value { get { return _value; } set { _value = value; } }
    }

    public static class ParamDescriptorExtensions
    {
        private static NumberStyles _numberFormat = NumberStyles.Float;
        private static string[] _formats = new string[] { "dd.MM.yyyy" };

        public static decimal ConvertNum(this ParamDescriptor paramDescriptor)
        {
            var value = decimal.Parse(paramDescriptor.Value.ToString(), _numberFormat);
            return value;
        }

        public static string ConvertStr(this ParamDescriptor paramDescriptor)
        {
            if (paramDescriptor.Value == null)
                return null;
            return paramDescriptor.Value.ToString();
        }

        public static DateTime ConvertDate(this ParamDescriptor paramDescriptor)
        {
            var value = DateTime.ParseExact(paramDescriptor.Value.ToString(),
                _formats, 
                null, 
                DateTimeStyles.None);

            return value;
        }

        public static DateTime ConvertDate(this ParamDescriptor paramDescriptor,
            string[] formats)
        {
            var value = DateTime.ParseExact(paramDescriptor.Value.ToString(),
                formats,
                null,
                DateTimeStyles.None);

            return value;
        }


        public static ParamDescriptor ConvertParam(ParamDescriptor oldParamDescriptor,
            ParamDescriptor newParamDescriptor)
        {
            if(oldParamDescriptor.ParamType == newParamDescriptor.ParamType)
            {
                oldParamDescriptor.Value = newParamDescriptor;
            }
            return oldParamDescriptor;
        }
    }

}
