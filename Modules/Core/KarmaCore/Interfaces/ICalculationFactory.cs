using KarmaCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Interfaces
{
    public interface ICalculationFactory
    {
        Calculation GetCalculation(CalculationJson json);
    }
}
