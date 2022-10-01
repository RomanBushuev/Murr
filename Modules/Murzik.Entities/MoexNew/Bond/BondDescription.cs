using System;

namespace Murzik.Entities.MoexNew.Bond
{
    public class BondDescription
    {
        public string SecId { get; set; }

        public string IssueName { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string RegNumber { get; set; }

        public string Isin { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? MaturityDate { get; set; }

        public string BuyBackDate { get; set; }

        public decimal? InitialFaceValue { get; set; }

        public string FaceUnit { get; set; }

        public string LatName { get; set; }

        public DateTime? StartDateMoex { get; set; }

        public string HasProspectus { get; set; }

        public DateTime? DecisionDate { get; set; }

        public string IsConcessionAgreement { get; set; }

        public string HasDefault { get; set; }

        public string HasTechnicalDefault { get; set; }

        public string ProgramRegistryNumber { get; set; }

        public string EmitentMisMatchFirst { get; set; }

        public string EmitentMisMatchCur { get; set; }

        public bool? Earlyrepayment { get; set; }

        public decimal? ListLevel { get; set; }

        public decimal? DaysToRedemption { get; set; }

        public decimal? IssueSize { get;set; }

        public decimal? FaceValue { get; set; }

        public bool? IsQualifiedInvestors { get; set; }

        public string QualInvestorGroup { get; set; }

        public decimal? CouponFrequency { get; set; }

        public DateTime? CouponDate { get; set; }

        public decimal? CouponPercent { get; set; }

        public decimal? CouponValue { get; set; }

        public decimal? IssueSizePlanned { get;set;}

        public string SecSubType { get; set; }

        public string MorningSession { get; set; }

        public string EveningSession { get; set; }

        public bool? HighRisk { get; set; }

        public string Type { get; set; }

        public string TypeName { get; set; }
    }
}
