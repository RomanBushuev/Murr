using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Utils
{
    public class Column
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int? Bytes { get; set; }

        public int? MaxSize { get; set; }
    }
}
