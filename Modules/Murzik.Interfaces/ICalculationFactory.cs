using Murzik.Entities;

namespace Murzik.Interfaces
{
    public interface ICalculationFactory
    {
        IAlgorithm GetCalculation(CalculationJson json);
    }
}
