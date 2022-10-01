using Murzik.Entities;
using Murzik.Entities.Enumerations;
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
    public class DownloadMoexInstrumentDescription : Algorithm
    {
        private IMoexDownloader _moexDownloader;
        public const string IsBond = "IsBond";
        public const string IsShare = "IsShare";
        public const string IsEurobond = "IsEurobond";
        public const string RunDateTime = "RunDateTime";

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
                },
                new ParamDescriptor()
                {
                    Ident = RunDateTime,
                    Description = "Дата",
                    ParamType = ParamType.DateTime,
                    Value = DateTime.Today.Date
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
                {
                    var bondFileName = await DownloadBonds();
                    GenerateBondDescriptionTask(bondFileName);
                }
                else
                    Log.Info($"Задача {TaskId} : Задача загрузки облигаций не задана");

                var isEurobond = GetBool(_paramDescriptors.ConvertStr(IsEurobond));
                if (isEurobond)
                {
                    var eurobondFileName = await DownloadEurobonds();
                    GenerateBondDescriptionTask(eurobondFileName);
                }
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

        /// <summary>
        /// формирование задачи на исполнение
        /// </summary>
        /// <param name="filename"></param>
        private void GenerateBondDescriptionTask(string filename)
        {
            var date = _paramDescriptors.ConvertDate(RunDateTime);
            var variables = ImportBondDescriptions.InitializeParams();
            variables.SetStr(ImportBondDescriptions.File, filename);
            variables.SetDat(ImportBondDescriptions.RunDateTime, date);

            var json = variables.SerializeJson();
            Log.Info($"Создаем задачу {TaskTypes.ImportBondDescription} с параметрами {json}");
            var task = TaskAction.GetKarmaDownloadJob().FirstOrDefault(z => z.TaskId == TaskId);
            var template = TaskAction.GetCalculationJson(task.TaskTemplateId);
            var newTaskId = TaskAction.CreateTaskAction(TaskStatuses.Creating,
            TaskTypes.ImportBondDescription.ToDbAttribute(),
            template.TaskTemplateFolderId,
            json,
            TaskTypes.ImportBondDescription);

            TaskAction.InsertPipelineTasks(TaskId, newTaskId);
            Log.Info($"Создали зависимость между {TaskId}->{newTaskId}");
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
