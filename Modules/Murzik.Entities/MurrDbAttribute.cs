using System;

namespace Murzik.Entities
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
