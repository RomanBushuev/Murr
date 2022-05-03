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
    public class DownloadMoexAmortizations : Algorithm
    {
        private IMoexDownloader _moexDownloader;
        private ICsvSaver _csvSaver;
        public const string Attempts = "Attempts";

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexAmortizations;

        public DownloadMoexAmortizations(ILogger logger,
            ITaskActions taskActions,
            IMoexDownloader moexDownloader,
            ICsvSaver csvSaver)
            : base(logger, taskActions)
        {
            _moexDownloader = moexDownloader;
            _csvSaver = csvSaver;
        }

        public override async Task Run()
        {
            await base.Run();

            try
            {
                Log.Info($"Задача {TaskId} : Задача загрузки амортизаций из Moex");
                IsContinue();
                var attempts = _paramDescriptors.ConvertNum(Attempts);
                long? amountOfPageToDownload = null;
                if(attempts != decimal.MinusOne)
                {
                    amountOfPageToDownload = (long)attempts;
                    Log.Info($"Количество страниц для загрузки {amountOfPageToDownload}");
                }
                var amortizations = await _moexDownloader.DownloadAmortizationsAsync(amountOfPageToDownload);
                Log.Info($"Задача {TaskId} : Количество загруженных купонов: {amortizations.Count}");
                var filepath = _csvSaver.Save(amortizations, TaskId);
                Log.Info($"Задача {TaskId} : Данные загружены в {filepath}");
                Log.Info($"Задача {TaskId} : Задача загрузки купонов из Moex закончена");
                Finished();
            }
            catch(OperationCanceledException)
            {
                Log.Info($"Задача отменена {TaskId}");
            }
            catch(Exception ex)
            {
                Log.Info($"Задача завершилась с ошибкой {TaskId}");
                Log.Error(ex);
            }
            finally
            {
                IsAliveTokenSource.Cancel();
            }
        }

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
                return _paramDescriptors;
            else
            {
                _paramDescriptors = new List<ParamDescriptor>();
                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = Attempts,
                    Description = "Попытки запуска",
                    ParamType = ParamType.Decimal,
                    Value = decimal.MinusOne
                });
                return _paramDescriptors;
            }
        }
    }
}
