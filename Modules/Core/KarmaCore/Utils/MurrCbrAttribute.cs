using System;
using System.Collections.Generic;
using System.Text;

namespace KarmaCore.Utils
{
    public class MurrCbrAttribute : Attribute
    {
        private string _method;
        
        public MurrCbrAttribute(string method)
        {
            _method = method;
        }
    }
}
