﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.Entities.MoexNew.Amortization
{
    public class AmortizationCursor
    {
        public long Index { get; set; }

        public long Total { get; set; }

        public long PageSize { get; set; }
    }
}
