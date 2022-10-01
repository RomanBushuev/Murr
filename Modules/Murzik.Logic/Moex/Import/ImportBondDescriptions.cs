using Murzik.Entities;
using Murzik.Entities.MoexNew.Packs;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Moex.Import
{
    public class ImportBondDescriptions : Algorithm
    {
        private IConverterFactory _converterFactory;
        private ISaverMurrData _saver;
        private ICsvReaderAgent _csvReaderAgent;
        public const string RunDateTime = "RunDateTime";
        public const string File = "File";

        public ImportBondDescriptions(ILogger logger, 
            ITaskActions taskAction,
            IConverterFactory converterFactory,
            ISaverMurrData saver,
            ICsvReaderAgent csvReaderAgent) : base(logger, taskAction)
        {
            _converterFactory = converterFactory;
            _saver = saver;
            _csvReaderAgent = csvReaderAgent;
        }

        public override TaskTypes TaskTypes => TaskTypes.ImportBondDescription;

        public override IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            if (_paramDescriptors is null)
                _paramDescriptors = InitializeParams();
            return _paramDescriptors;
        }

        public static List<ParamDescriptor> InitializeParams()
        {
            var paramDescriptors = new List<ParamDescriptor>();
            paramDescriptors.Add(new ParamDescriptor()
            {
                Ident = RunDateTime,
                Description = "Время для запуска",
                ParamType = ParamType.DateTime,
                Value = DateTime.Today.Date
            });
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
                Log.Info($"Задача {TaskId} : Загрузка данных по облигациям");
                var path = _paramDescriptors.ConvertStr(File);
                var date = _paramDescriptors.ConvertDate(RunDateTime);
                Log.Info($"Задача {TaskId}. Файл для загрузки: {path}");
                Log.Info($"Задача {TaskId}. Дата загрузки: {date}");

                var bondDescriptions = await _csvReaderAgent.ReadBondDescriptionAsync(path);
                Log.Info($"Задача {TaskId}. Количество загруженных данных: {bondDescriptions.Count}");
                var pack = new PackMoexBonds()
                {
                    Bonds = bondDescriptions,
                    ValidDate = date,
                };
                Log.Info($"Задача {TaskId} : Конвертация данных");
                var converter = _converterFactory.GetConverter(typeof(PackMoexBonds));
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
