using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Moex.Download
{
    public class DownloadMoexOffers : Algorithm
    {
        private IMoexDownloader _moexDownloader;
        private ICsvSaver _csvSaver;
        public const string Attempts = "Attempts";

        public DownloadMoexOffers(ILogger logger,
            ITaskActions taskActions,
            IMoexDownloader moexDownloader,
            ICsvSaver csvSaver)
            : base(logger, taskActions)
        {
            _moexDownloader = moexDownloader;
            _csvSaver = csvSaver;
        }

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexOffers;

        public override async Task Run()
        {
            await base.Run();

            try
            {
                Log.Info($"Задача {TaskId} : Задача загрузки купонов из Moex");
                IsContinue();
                decimal attemps = _paramDescriptors.ConvertNum(Attempts);
                long? amountOfPageToDownload = null;
                if (attemps != decimal.MinusOne)
                {
                    amountOfPageToDownload = (long)attemps;
                    Log.Info($"Количество страниц для загрузки {amountOfPageToDownload}");
                }
                var offers = await _moexDownloader.DownloadOffersAsync(amountOfPageToDownload);
                Log.Info($"Задача {TaskId} : Количество загруженных купонов: {offers.Count}");
                var filepath = _csvSaver.Save(offers, TaskId);
                Log.Info($"Задача {TaskId} : Данные загружены в {filepath}");
                Log.Info($"Задача {TaskId} : Задача загрузки купонов из Moex закончена");
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
