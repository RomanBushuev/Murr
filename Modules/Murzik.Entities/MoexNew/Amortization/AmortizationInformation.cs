namespace Murzik.Entities.MoexNew.Amortization
{
    public class AmortizationInformation
    {
        public AmortizationCursor[] AmortizationCursors { get; set; } = new AmortizationCursor[] { };

        public Amortization[] Amortizations { get; set; } = new Amortization[] { };
    }
}
