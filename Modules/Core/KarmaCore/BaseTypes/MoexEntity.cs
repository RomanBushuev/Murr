using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.BaseTypes
{
    public class MoexEntity
    {
        public string Ident { get; set; }

        public MoexFinTypes MoexFinType { get; set; }
        
        public Dictionary<ScalarAttribute, ScalarDate> ScalarDate { get; set; }

        public Dictionary<ScalarAttribute, ScalarStr> ScalarStr { get; set; }

        public Dictionary<ScalarAttribute, ScalarNum> ScalarNum { get; set; }

        public Dictionary<ScalarAttribute, TimeSeries> TimeSeries { get; set; }
    }
}
