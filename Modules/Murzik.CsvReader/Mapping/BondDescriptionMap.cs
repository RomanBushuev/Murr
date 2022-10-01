using CsvHelper.Configuration;
using Murzik.Entities.MoexNew.Bond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murzik.CsvReaderProvider.Mapping
{
    internal class BondDescriptionMap : ClassMap<BondDescription>
    {
        public BondDescriptionMap()
        {
            Map(z => z.SecId).Name("SECID");
            Map(z => z.IssueName).Name("ISSUENAME");
            Map(z => z.Name).Name("NAME");
            Map(z => z.ShortName).Name("SHORTNAME");
            Map(z => z.RegNumber).Name("REGNUMBER");
            Map(z => z.Isin).Name("ISIN");
            Map(z => z.IssueDate).Name("ISSUEDATE");
            Map(z => z.MaturityDate).Name("MATDATE");
            Map(z => z.BuyBackDate).Name("BUYBACKDATE");
            Map(z => z.InitialFaceValue).Name("INITIALFACEVALUE");
            Map(z => z.FaceUnit).Name("FACEUNIT");
            Map(z => z.LatName).Name("LATNAME");
            Map(z => z.StartDateMoex).Name("STARTDATEMOEX").Optional();
            Map(z => z.HasProspectus).Name("HASPROSPECTUS");
            Map(z => z.DecisionDate).Name("DECISIONDATE");
            Map(z => z.IsConcessionAgreement).Name("ISCONCESSIONAGREEMENT");
            Map(z => z.HasDefault).Name("HASDEFAULT");
            Map(z => z.HasTechnicalDefault).Name("HASTECHNICALDEFAULT");
            Map(z => z.ProgramRegistryNumber).Name("PROGRAMREGISTRYNUMBER");
            Map(z => z.EmitentMisMatchFirst).Name("EMITENTMISMATCHFIRST");
            Map(z => z.EmitentMisMatchCur).Name("EMITENTMISMATCHCUR");
            Map(z => z.Earlyrepayment).Name("EARLYREPAYMENT").Optional();
            Map(z => z.ListLevel).Name("LISTLEVEL");
            Map(z => z.DaysToRedemption).Name("DAYSTOREDEMPTION");
            Map(z => z.IssueSize).Name("ISSUESIZE").Optional();
            Map(z => z.FaceValue).Name("FACEVALUE").Optional();
            Map(z => z.IsQualifiedInvestors).Name("ISQUALIFIEDINVESTORS");
            Map(z => z.QualInvestorGroup).Name("QUALINVESTORGROUP");
            Map(z => z.CouponFrequency).Name("COUPONFREQUENCY");
            Map(z => z.CouponDate).Name("COUPONDATE");
            Map(z => z.CouponPercent).Name("COUPONPERCENT").Optional();
            Map(z => z.CouponValue).Name("COUPONVALUE");
            Map(z => z.IssueSizePlanned).Name("ISSUESIZEPLANNED").Optional();
            Map(z => z.SecSubType).Name("SECSUBTYPE").Optional();
            Map(z => z.MorningSession).Name("MORNINGSESSION");
            Map(z => z.EveningSession).Name("EVENINGSESSION");
            Map(z => z.HighRisk).Name("HIGHRISK");
            Map(z => z.Type).Name("TYPE");
            Map(z => z.TypeName).Name("TYPENAME");
        }
    }
}
