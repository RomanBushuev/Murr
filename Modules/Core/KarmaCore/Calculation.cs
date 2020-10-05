﻿using KarmaCore.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarmaCore
{
    /// <summary>
    /// Задача, расчет, все что угодно
    /// </summary>
    public class Calculation
    {
        private bool isInitialized = false;
        protected List<ParamDescriptor> _paramDescriptors = new List<ParamDescriptor>();

        public virtual void Run()
        {

        }

        public virtual IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            return _paramDescriptors;
        }


        public virtual void SetParamDescriptors(ParamDescriptor paramDescriptor)
        {
            if(!isInitialized)
            {
                GetParamDescriptors();
                isInitialized = true;
            }
            var needParam = _paramDescriptors.FirstOrDefault(z => z.Ident == paramDescriptor.Ident);
            needParam = ParamDescriptorExtensions.ConvertParam(needParam, paramDescriptor);
        }
    }
}
