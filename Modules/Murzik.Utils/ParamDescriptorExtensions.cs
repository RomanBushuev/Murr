using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Murzik.Utils
{
    public static class ParamDescriptorExtensions
    {
        private static NumberStyles _numberFormat = NumberStyles.Float;
        private static string[] _formats = new string[] { "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy H:mm:ss", "dd.MM.yyyy" };

        public static void SetNum(this IEnumerable<ParamDescriptor> paramDescriptors, string ident, decimal val)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            param.Value = val;
        }

        public static decimal ConvertNum(this ParamDescriptor paramDescriptor)
        {
            var value = (decimal)paramDescriptor.Value;
            return value;
        }

        public static void SetStr(this IEnumerable<ParamDescriptor> paramDescriptors, string ident, string val)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            param.Value = val;
        }

        public static string ConvertStr(this ParamDescriptor paramDescriptor)
        {
            if (paramDescriptor.Value == null)
                return null;
            return paramDescriptor.Value.ToString();
        }

        public static void SetDat(this IEnumerable<ParamDescriptor> paramDescriptors, string ident, DateTime val)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            param.Value = val;
        }

        public static DateTime ConvertDate(this ParamDescriptor paramDescriptor)
        {
            var value = (DateTime)paramDescriptor.Value;
            return value;
        }

        public static decimal ConvertNum(this IEnumerable<ParamDescriptor> paramDescriptors, string ident)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            return param != null ? param.ConvertNum() : throw new ArgumentException($"Не найден параметр {ident}");
        }

        public static DateTime ConvertDate(this IEnumerable<ParamDescriptor> paramDescriptors, string ident)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            return param != null ? param.ConvertDate() : throw new ArgumentException($"Не найден параметр {ident}");
        }

        public static string ConvertStr(this IEnumerable<ParamDescriptor> paramDescriptors, string ident)
        {
            var param = paramDescriptors.FirstOrDefault(z => z.Ident == ident);
            return param != null ? param.ConvertStr() : throw new ArgumentException($"Не найден параметр {ident}");
        }

        public static string SerializeJson(this IEnumerable<ParamDescriptor> paramDescriptors)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var val in paramDescriptors)
            {
                if (val.ParamType == ParamType.DateTime)
                {
                    var temp = (DateTime)val.Value;
                    if (temp.Hour != 0 || temp.Minute != 0 || temp.Second != 0)
                    {
                        string str = ((DateTime)val.Value).ToString("dd.MM.yyyy HH:mm:ss");
                        keyValuePairs[val.Ident] = str;
                    }
                    else
                    {
                        string str = ((DateTime)val.Value).ToString("yyyy-MM-dd");
                        keyValuePairs[val.Ident] = str;
                    }
                }
                else if (val.ParamType == ParamType.Decimal)
                {
                    string str = ((decimal)val.Value).ToString(CultureInfo.InvariantCulture);
                    keyValuePairs[val.Ident] = str;
                }
                else if (val.ParamType == ParamType.String)
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
            foreach (var val in paramDescriptors)
            {
                if(!dict.ContainsKey(val.Ident))
                {
                    values.Add(val);
                    continue;
                }
                if (val.ParamType == ParamType.DateTime)
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
                else if (val.ParamType == ParamType.Decimal)
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
                else if (val.ParamType == ParamType.String)
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
            if (oldParamDescriptor.ParamType == newParamDescriptor.ParamType)
            {
                oldParamDescriptor.Value = newParamDescriptor.Value;
            }
            return oldParamDescriptor;
        }
    }
}
