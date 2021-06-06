using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Utils
{
    public class MoexFinAttribute : Attribute
    {
        private string _ident;

        public MoexFinAttribute(string ident)
        {
            _ident = ident;
        }

        public string Ident => _ident;
    }
}
