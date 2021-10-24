using Murzik.Entities.Enumerations;
using System;

namespace Murzik.Entities
{
    public class ParamDescriptor
    {
        public string Ident { get; set; }

        public string Description { get; set; }

        public ParamType ParamType { get; set; }

        public object Value { get; set; }
    }
}
