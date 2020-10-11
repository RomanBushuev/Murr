using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Utils
{
    public class MurrDbAttribute : Attribute
    {
        private string _ident;

        public MurrDbAttribute(string ident)
        {
            _ident = ident;
        }

        public string Ident => _ident;
    }
}
