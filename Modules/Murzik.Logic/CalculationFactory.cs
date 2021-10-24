using Murzik.Entities;
using Murzik.Interfaces;
using System;

namespace Murzik.Logic
{
    public class CalculationFactory : ICalculationFactory
    {
        public IAlgorithm GetCalculation(CalculationJson json)
        {
            throw new NotImplementedException();
        }
    }
}
