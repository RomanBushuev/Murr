using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.Entities.MoexNew.Offer
{
    public class OfferCursor
    {
        public long Index { get; set; }

        public long Total { get; set; }

        public long PageSize { get; set; }
    }
}
