using CsvHelper.Configuration;
using Murzik.Entities.MoexNew.Share;

namespace Murzik.CsvProvider.Mapping
{
    internal class ShareDescriptionMap : ClassMap<ShareDescription>
    {
        public ShareDescriptionMap()
        {
            Map(z => z.SecId).Name("SECID");
            Map(z => z.IssueName).Name("ISSUENAME");
            Map(z => z.Name).Name("NAME");
            Map(z => z.ShortName).Name("SHORTNAME");
            Map(z => z.Isin).Name("ISIN");
            Map(z => z.RegNumber).Name("REGNUMBER");
            Map(z => z.IssueSize).Name("ISSUESIZE");
            Map(z => z.FaceValue).Name("FACEVALUE");
            Map(z => z.FaceUnit).Name("FACEUNIT");
            Map(z => z.IssueDate).Name("ISSUEDATE");
            Map(z => z.LatName).Name("LATNAME");
            Map(z => z.DepositaryNoteType).Name("DEPOSITARYNOTETYPE");
            Map(z => z.HasProspectus).Name("HASPROSPECTUS");
            Map(z => z.DecisionDate).Name("DECISIONDATE");
            Map(z => z.HasDefault).Name("HASDEFAULT");
            Map(z => z.HasTechnicalDefault).Name("HASTECHNICALDEFAULT");
            Map(z => z.EmitentMisMatchFirst).Name("EMITENTMISMATCHFIRST");
            Map(z => z.EmitentMisMatchCur).Name("EMITENTMISMATCHCUR");
            Map(z => z.ListLevel).Name("LISTLEVEL");
            Map(z => z.IsQualifiedInvestors).Name("ISQUALIFIEDINVESTORS");
            Map(z => z.QualInvestorGroup).Name("QUALINVESTORGROUP");
            Map(z => z.SharesPerReceipt).Name("SHARESPERRECEIPT");
            Map(z => z.MorningSession).Name("MORNINGSESSION");
            Map(z => z.EveningSession).Name("EVENINGSESSION");
            Map(z => z.HighRisk).Name("HIGHRISK");
            Map(z => z.Type).Name("TYPE");
            Map(z => z.TypeName).Name("TYPENAME");
        }
    }
}
