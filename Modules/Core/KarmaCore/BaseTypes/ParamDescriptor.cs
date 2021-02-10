using KarmaCore.Enumerations;
using KarmaCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

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
        private static string[] _formats = new string[] { "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy H:mm:ss", "dd.MM.yyyy" };

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

        public static DateTime ConvertDate(this IEnumerable<ParamDescriptor> paramDescriptors, string ident)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            if (param != null)
            {
                return param.ConvertDate();
            }
            else
            {
                throw new KarmaCoreException($"Не найден параметр {ident}");
            }
        }

        public static string ConvertStr(this IEnumerable<ParamDescriptor> paramDescriptors, string ident)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            if (param != null)
            {
                return param.ConvertStr();
            }
            else
            {
                throw new KarmaCoreException($"Не найден параметр {ident}");
            }
        }


        public static string SerializeJson(this IEnumerable<ParamDescriptor> paramDescriptors)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach(var val in paramDescriptors)
            {
                if(val.ParamType == ParamType.DateTime)
                {
                    string str = ((DateTime)val.Value).ToString("yyyy-MM-dd");
                    keyValuePairs[val.Ident] = str;
                }
                else if(val.ParamType == ParamType.Decimal)
                {
                    string str = ((decimal)val.Value).ToString(CultureInfo.InvariantCulture);
                    keyValuePairs[val.Ident] = str;
                }
                else if(val.ParamType == ParamType.String)
                {
                    keyValuePairs[val.Ident] = val.Value.ToString();
                }
            }
            return JsonConvert.SerializeObject(keyValuePairs);
        }

        public static List<ParamDescriptor> DeserializeJson(string json, IEnumerable<ParamDescriptor> paramDescriptors)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            List<ParamDescriptor> values = new List<ParamDescriptor>();
            foreach(var val in paramDescriptors)
            {
                if(val.ParamType == ParamType.DateTime)
                {
                    string str = dict[val.Ident];
                    DateTime dateTime = DateTime.ParseExact(str, _formats, null);
                    values.Add(new ParamDescriptor()
                    {
                        Ident = val.Ident,
                        Description = val.Description,
                        ParamType = val.ParamType,
                        Value = dateTime
                    });
                }
                else if(val.ParamType == ParamType.Decimal)
                {
                    string str = dict[val.Ident];
                    decimal v = decimal.Parse(str, CultureInfo.InvariantCulture);
                    values.Add(new ParamDescriptor()
                    {
                        Ident = val.Ident,
                        Description = val.Description,
                        ParamType = val.ParamType,
                        Value = v
                    });
                }
                else if(val.ParamType == ParamType.String)
                {
                    string str = dict[val.Ident];
                    values.Add(new ParamDescriptor()
                    {
                        Ident = val.Ident,
                        Description = val.Description,
                        ParamType = val.ParamType,
                        Value = str
                    });
                }
            }

            return values;
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
                oldParamDescriptor.Value = newParamDescriptor.Value;
            }
            return oldParamDescriptor;
        }
    }

}
