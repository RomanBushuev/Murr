using CsvHelper.Configuration;
using Murzik.Entities.MoexNew.Bond;

namespace Murzik.CsvProvider.Mapping
{
    internal class BondDataRowMap : ClassMap<BondDataRow>
    {
        public BondDataRowMap()
        {
            Map(z => z.Accint).Name("Accint");
            Map(z => z.AdmittedQuote).Name("AdmittedQuote");
            Map(z => z.AdmittedValue).Name("AdmittedValue");
            Map(z => z.BeiClose).Name("BeiClose");
            Map(z => z.BoardId).Name("BoardId");
            Map(z => z.BuyBackDate).Name("BuyBackDate");
            Map(z => z.CbrClose).Name("CbrClose");
            Map(z => z.Close).Name("Close");
            Map(z => z.CouponPercent).Name("CouponPercent");
            Map(z => z.CouponValue).Name("CouponValue");
            Map(z => z.CurrencyId).Name("CurrencyId");
            Map(z => z.Duration).Name("Duration");
            Map(z => z.FaceUnit).Name("FaceUnit");
            Map(z => z.FaceValue).Name("FaceValue");
            Map(z => z.High).Name("High");
            Map(z => z.IriCPiClose).Name("IriCPiClose");
            Map(z => z.LastTradeDate).Name("LastTradeDate");
            Map(z => z.LegalClosePrice).Name("LegalClosePrice");
            Map(z => z.Low).Name("Low");
            Map(z => z.MarketPrice2).Name("MarketPrice2");
            Map(z => z.MarketPrice3).Name("MarketPrice3");
            Map(z => z.MarketPrice3TradesValue).Name("MarketPrice3TradesValue");
            Map(z => z.MatDate).Name("MatDate");
            Map(z => z.MP2ValTrd).Name("MP2ValTrd");
            Map(z => z.NumTrades).Name("NumTrades");
            Map(z => z.OfferDate).Name("OfferDate");
            Map(z => z.Open).Name("Open");
            Map(z => z.Secid).Name("Secid");
            Map(z => z.Shortname).Name("Shortname");
            Map(z => z.TradeDate).Name("TradeDate");
            Map(z => z.TradingSession).Name("TradingSession");
            Map(z => z.Value).Name("Value");
            Map(z => z.Volume).Name("Volume");
            Map(z => z.Waprice).Name("Waprice");
            Map(z => z.YieldAtWap).Name("YieldAtWap");
            Map(z => z.YieldClose).Name("YieldClose");
            Map(z => z.YieldLastCoupon).Name("YieldLastCoupon");
            Map(z => z.YieldToOffer).Name("YieldToOffer");
        }
    }
}
