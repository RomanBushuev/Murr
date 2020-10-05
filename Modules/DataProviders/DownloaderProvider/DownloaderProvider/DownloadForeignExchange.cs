using KarmaCore;
using KarmaCore.BaseTypes;
using KarmaCore.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider
{
    public class DownloadForeignExchange : Calculation
    {
        public string RunDateTime = "RunDateTime";

        public override void Run()
        {
            base.Run();
        }

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            _paramDescriptors.Clear();
            _paramDescriptors.Add(new ParamDescriptor()
            {
                Ident = RunDateTime,
                Description = "Время для запуска",
                ParamType = ParamType.DateTime,
                Value = DateTime.Today.Date
            });

            return _paramDescriptors;
        }
    }
}
