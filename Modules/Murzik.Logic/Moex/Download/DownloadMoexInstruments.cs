using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using Murzik.Interfaces;
using Murzik.Logic.Moex.Import;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murzik.Logic.Moex.Download
{
    public class DownloadMoexInstruments : Algorithm
    {
        public const string RunDateTime = "RunDateTime";
        public const string InstrumentType = "InstrumentType";
        public IMoexDownloader _moexProvider;
        private IXmlSaver _xmlSaver;
        private ICsvSaver _csvSaver;

        public DownloadMoexInstruments(ILogger logger,
            ITaskActions taskAction,
            IMoexDownloader moexProvider,
            IXmlSaver xmlSaver,
            ICsvSaver csvSaver) : base(logger, taskAction)
        {
            _moexProvider = moexProvider;
            _xmlSaver = xmlSaver;
            _csvSaver = csvSaver;
        }

        public override TaskTypes TaskTypes => TaskTypes.DownloadMoexInstruments;

        public override async Task Run()
        {
            await base.Run();

            try
            {
                Log.Info($"Задача {TaskId} : Задача финансовых инструментов начата");
                IsContinue();
                var typeInstrument = _paramDescriptors.ConvertStr(InstrumentType);
                var date = _paramDescriptors.ConvertDate(RunDateTime);
                if (typeInstrument == "shares")
                {
                    Log.Info($"Задача {TaskId} : загрузка акций");
                    var shares = await DownloadShares(date);
                    IsContinue();
                    Log.Info($"Задача {TaskId} : загрузка акций завершена");

                    var saverJson = TaskAction.GetSaverJson(TaskId);
                    if (saverJson is not null)
                    {
                        Log.Info($"Задача {TaskId} : сохранение акций");
                        var connection = _xmlSaver.Deserialize(saverJson).Connection;
                        connection = connection.Replace(".xml", ".csv");
                        _csvSaver.Save(shares, connection);
                        Log.Info($"Задача {TaskId} : закончена сохранение акций");
                    }
                }

                if (typeInstrument == "bonds")
                {
                    Log.Info($"Задача {TaskId} : загрузка облигаций");
                    var bonds = await DownloadBonds(date);
                    IsContinue();
                    Log.Info($"Задача {TaskId} : загрузка облигаций завершена");

                    var saverJson = TaskAction.GetSaverJson(TaskId);
                    if (saverJson is not null)
                    {
                        Log.Info($"Задача {TaskId} : сохранение облигаций");
                        var connection = _xmlSaver.Deserialize(saverJson).Connection;
                        connection = connection.Replace(".xml", ".csv");
                        _csvSaver.Save(bonds, connection);
                        Log.Info($"Задача {TaskId} : закончена сохранение облигаций");
                        GenerateBondQuoteTask(connection);
                    }
                }

                IsContinue();
                Log.Info($"Задача {TaskId} : Задача финансовых инструментов закончена");
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

        /// <summary>
        /// формирование задачи на исполнение
        /// </summary>
        /// <param name="filename"></param>
        private void GenerateBondQuoteTask(string filename)
        {
            var variables = ImportBondQuotes.InitializeParams();
            variables.SetStr(ImportBondQuotes.File, filename);

            var json = variables.SerializeJson();
            Log.Info($"Создаем задачу {TaskTypes.ImportBondQuotes} с параметрами {json}");
            var task = TaskAction.GetKarmaDownloadJob().FirstOrDefault(z => z.TaskId == TaskId);
            var template = TaskAction.GetCalculationJson(task.TaskTemplateId);
            var newTaskId = TaskAction.CreateTaskAction(TaskStatuses.Creating,
                TaskTypes.ImportBondQuotes.ToDbAttribute(),
                template.TaskTemplateFolderId,
                json,
                TaskTypes.ImportBondQuotes);

            TaskAction.InsertPipelineTasks(TaskId, newTaskId);
            Log.Info($"Создали зависимость между {TaskId}->{newTaskId}");
        }

        private async Task<IReadOnlyCollection<ShareDataRow>> DownloadShares(DateTime date)
        {
            var shares = await _moexProvider.DownloadShareDataRowAsync(date);
            return shares;
        }

        private async Task<IReadOnlyCollection<BondDataRow>> DownloadBonds(DateTime date)
        {
            var bonds = await _moexProvider.DownloadBondDataRowAsync(date);
            return bonds;
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
                    Ident = RunDateTime,
                    Description = "Время для запуска",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
                });

                _paramDescriptors.Add(new ParamDescriptor()
                {
                    Ident = InstrumentType,
                    Description = "Тип инструмента для загрузку: index, shares, bonds, ndm, otc, ccp, deposit, repo, qnv, mamc, foreignshares, foreignndm, moexboard, gcc, credit, standard, classica",
                    ParamType = ParamType.String,
                    Value = "shares"
                });

                return _paramDescriptors;
            }
        }
    }
}
