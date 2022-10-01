using Murzik.Entities;
using Murzik.Entities.Cbr.Packs;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Cbr.Import
{
    public class SaveForeignExchange : Algorithm
    {
        private IConverterFactory _converterFactory;
        private ISaverMurrData _saver;
        private ICbrDownloader _cbrDownloader;
        public const string RunDateTime = "RunDateTime";
        public const string File = "File";

        public SaveForeignExchange(ILogger logger,
            ITaskActions taskActions,
            IConverterFactory converterFactory,
            ISaverMurrData saver,
            ICbrDownloader cbrDownloader)
            : base(logger, taskActions)
        {
            _converterFactory = converterFactory;
            _saver = saver;
            _cbrDownloader = cbrDownloader;
        }

        public override TaskTypes TaskTypes => TaskTypes.SaveForeignExchange;

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors != null)
                return _paramDescriptors;
            else
            {
                _paramDescriptors = new List<ParamDescriptor>();
                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = RunDateTime,
                    Description = "Время для запуска",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
                });
                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = File,
                    Description = "Путь к файлу",
                    ParamType = ParamType.String,
                    Value = string.Empty
                });

                return _paramDescriptors;
            }
        }

        public async override Task Run()
        {
            await base.Run();

            try
            {
                IsContinue();
                var path = _paramDescriptors.ConvertStr(File);
                var date = _paramDescriptors.ConvertDate(RunDateTime);

                Log.Info($"Задача {TaskId} : Загрузка данных");
                var currencies = _cbrDownloader.DownloadCurrencies(path);
                var pack = new PackCurrencies()
                {
                    ValidDate = date,
                    Currencies = currencies
                };

                Log.Info($"Задача {TaskId} : Конвертация данных");
                var converter = _converterFactory.GetConverter(typeof(PackCurrencies));
                var importedDate = converter.ConvertToPackValues(pack);
                IsContinue();

                Log.Info($"Задача {TaskId} : Сохранение данных");
                _saver.Save(importedDate);

                Finished();
            }
            catch (OperationCanceledException)
            {
                Log.Info($"Задача отменена {TaskId}");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                IsAliveTokenSource.Cancel();
            }
        }
    }
}
