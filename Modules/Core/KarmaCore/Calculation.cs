using KarmaCore.BaseTypes;
using KarmaCore.BaseTypes.Logger;
using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarmaCore
{
    /// <summary>
    /// Задача, расчет, все что угодно
    /// </summary>
    public class Calculation : IResult
    {
        private bool isInitialized = false;
        protected List<ParamDescriptor> _paramDescriptors = null;
        protected MurrLogger _murrLogger = new MurrLogger();
        protected KarmaEnvironment _karmaEnvironment = new KarmaEnvironment();

        public virtual TaskTypes TaskTypes { get; }

        public KarmaEnvironment KarmaEnvironment { get { return _karmaEnvironment; } }

        public virtual void Run()
        {

        }

        public virtual IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            return _paramDescriptors;
        }


        public virtual void SetParamDescriptors(ParamDescriptor paramDescriptor)
        {
            if(_paramDescriptors == null)
            {
                GetParamDescriptors();
            }
            var needParam = _paramDescriptors.FirstOrDefault(z => z.Ident == paramDescriptor.Ident);
            needParam = ParamDescriptorExtensions.ConvertParam(needParam, paramDescriptor);
        }

        public void Notify(string message, MurrMessageType murrMessageType = MurrMessageType.Information) 
        {
            _murrLogger.Notify(message, murrMessageType);
        }

        public virtual CalculationJson Serialize()
        {
            throw new Exception("");
        }
    }
}
