using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.Entities.MoexNew.Offer
{
    public class OfferInformation
    {
        public OfferCursor[] OfferCursors { get; set; } = new OfferCursor[] { };

        public Offer[] Offers { get; set; } = new Offer[] { };
    }
}
