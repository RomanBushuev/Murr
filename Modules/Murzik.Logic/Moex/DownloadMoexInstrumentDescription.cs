using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Moex
{
    public class DownloadMoexInstrumentDescription : Algorithm
    {
        private IMoexDownloader _moexDownloader;
        public const string IsBond = "IsBond";
        public const string IsShare = "IsShare";
        public const string IsEurobond = "IsEurobond";

        public DownloadMoexInstrumentDescription(ILogger logger,
            ITaskActions taskAction,
            IMoexDownloader moexDownloader) : base(logger, taskAction)
        {
            _moexDownloader = moexDownloader;
        }

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexInstrumentDescription;

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
                return _paramDescriptors;

            _paramDescriptors = new List<ParamDescriptor>()
            {
                new ParamDescriptor()
                {
                    Ident = IsBond,
                    Description = "Облигации",
                    ParamType = ParamType.String,
                    Value = "Y"
                },
                new ParamDescriptor()
                {
                    Ident = IsShare,
                    Description = "Акции",
                    ParamType = ParamType.String,
                    Value = "Y"
                },
                new ParamDescriptor()
                {
                    Ident = IsEurobond,
                    Description = "Еврооблигации",
                    ParamType = ParamType.String,
                    Value = "Y"
                }
            };
            return _paramDescriptors;
        }

        public override async Task Run()
        {
            await base.Run();

            try
            {
                Log.Info($"Задача {TaskId} : Задача загрузки опистальных данных из Moex");
                IsContinue();

                var isShare = GetBool(_paramDescriptors.ConvertStr(IsShare));
                if (isShare)
                    await DownloadShare();
                else
                    Log.Info($"Задача {TaskId} : Задача загрузки акций не задана");

                var isBond = GetBool(_paramDescriptors.ConvertStr(IsBond));
                if (isBond)
                    await DownloadBonds();
                else
                    Log.Info($"Задача {TaskId} : Задача загрузки облигаций не задана");

                var isEurobond = GetBool(_paramDescriptors.ConvertStr(IsEurobond));
                if (isEurobond)
                    await DownloadEurobonds();
                else
                    Log.Info($"Задача {TaskId} : Задача загрузки еврооблигаций не задана");
                Log.Info($"Задача {TaskId} : Задача загрузки опистальных данных из Moex закончена");
                Finished();
            }
            catch (OperationCanceledException)
            {
                Log.Info($"Задача отменена {TaskId}");
            }
            catch (Exception ex)
            {
                Log.Info($"Задача завершилась с ошибкой {TaskId}");
                Log.Error(ex);
            }
            finally
            {
                IsAliveTokenSource.Cancel();
            }
        }

        private async Task<string> DownloadShare()
        {
            Log.Info($"Задача {TaskId} : Задача загрузки описательных данных по акциям из Moex");
            return await _moexDownloader.DownloadShareDescriptionAsync();
        }

        private async Task<string> DownloadBonds()
        {
            Log.Info($"Задача {TaskId} : Задача загрузки описательных данных по облигациям из Moex");
            return await _moexDownloader.DownloadBondDescriptionAsync();
        }

        private async Task<string> DownloadEurobonds()
        {
            Log.Info($"Задача {TaskId} : Задача загрузки описательных данных по еврооблигациям из Moex");
            return await _moexDownloader.DownloadEurobondDescriptionAsync();
        }

        private bool GetBool(string value) =>
            value.ToUpper().Equals("Y");
    }
}
