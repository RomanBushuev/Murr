using KarmaCore.BaseTypes;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoexDataProvider
{
    public class MoexMarketProvider : IMarketProvider
    {
        public ScalarDate GetScalarDate(string ident, ScalarAttribute scalarAttribute)
        {
            throw new NotImplementedException();
        }

        public ScalarEnum GetScalarEnum(string ident, ScalarAttribute scalarAttribute)
        {
            throw new NotImplementedException();
        }

        public ScalarNum GetScalarNum(string ident, ScalarAttribute scalarAttribute)
        {
            throw new NotImplementedException();
        }

        public ScalarStr GetScalarStr(string ident, ScalarAttribute scalarAttribute)
        {
            throw new NotImplementedException();
        }
    }
}
