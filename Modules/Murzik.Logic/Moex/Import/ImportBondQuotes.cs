using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Moex.Import
{
    public class ImportBondQuotes : Algorithm
    {
        private IConverterFactory _converterFactory;
        private ISaverMurrData _saver;
        private ICsvReaderAgent _csvReaderAgent;
        public const string File = "File";

        public ImportBondQuotes(ILogger logger,
            ITaskActions taskAction,
            IConverterFactory converterFactory,
            ISaverMurrData saver,
            ICsvReaderAgent csvReaderAgent) : base(logger, taskAction)
        {
            _converterFactory = converterFactory;
            _saver = saver;
            _csvReaderAgent = csvReaderAgent;
        }

        public override TaskTypes TaskTypes => TaskTypes.ImportBondQuotes;

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors() =>
            _paramDescriptors = _paramDescriptors is null 
            ? InitializeParams() 
            : _paramDescriptors;

        public static List<ParamDescriptor> InitializeParams()
        {
            var paramDescriptors = new List<ParamDescriptor>();
            paramDescriptors.Add(new ParamDescriptor()
            {
                Ident = File,
                Description = "Путь к файлу",
                ParamType = ParamType.String,
                Value = string.Empty
            });

            return paramDescriptors;
        }

        public async override Task Run()
        {
            await base.Run();

            try
            {
                IsContinue();
                Log.Info($"Задача {TaskId} : Загрузка котировальных данных по облигациям");
                var path = _paramDescriptors.ConvertStr(File);
                Log.Info($"Задача {TaskId}. Файл для загрузки: {path}");

                var bondDescriptions = await _csvReaderAgent.ReadBondDataRawAsync(path);
                Log.Info($"Задача {TaskId}. Количество загруженных данных: {bondDescriptions.Count}");
                var pack = new PackMoexBondQuote()
                {
                    Bonds = bondDescriptions,
                    QuoteSources = new List<string>() { "TQCB", "TQOB" }
                };
                Log.Info($"Задача {TaskId} : Конвертация данных");
                var converter = _converterFactory.GetConverter(typeof(PackMoexBondQuote));
                var importedDate = converter.ConvertToPackValues(pack);
                Log.Info($"Задача {TaskId} : Конвертация данных завершена");
                IsContinue();

                Log.Info($"Задача {TaskId} : Сохранение данных");
                _saver.Save(importedDate);
                Log.Info($"Задача {TaskId} : Сохранение данных завершена");

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
