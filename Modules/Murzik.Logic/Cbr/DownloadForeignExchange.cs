using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murzik.Logic.Cbr
{
    public class DownloadForeignExchange : Algorithm
    {
        private ICbrDownloader _cbrDownloader;
        private IXmlSaver _xmlSaver;
        public const string RunDateTime = "RunDateTime";

        public DownloadForeignExchange(ILogger logger,
            ITaskActions taskAction,
            ICbrDownloader cbrDownloader,
            IXmlSaver xmlSaver) : base(logger, taskAction)
        {
            _cbrDownloader = cbrDownloader;
            _xmlSaver = xmlSaver;
        }

        public override TaskTypes TaskTypes => TaskTypes.DownloadCurrenciesCbrf;

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

                return _paramDescriptors;
            }
        }

        public async override Task Run()
        {
            await base.Run();

            try
            {
                IsContinue();
                DateTime dateTime = _paramDescriptors.ConvertDate(RunDateTime);
                Log.Info($"Задача {TaskId} : загрузка валют за дату {dateTime.ToShortDateString()}");
                var xmlDocument = await _cbrDownloader.DownloadForeignExchange(dateTime);
                Log.Info($"Задача {TaskId} : загрузка валют завершена");

                IsContinue();
                var saverJson = TaskAction.GetSaverJson(TaskId);
                if (saverJson is not null)
                {
                    Log.Info($"Задача {TaskId} : сохранение значений");
                    _xmlSaver.Deserialize(saverJson).Save(xmlDocument);
                    Log.Info($"Задача {TaskId} : значения сохранены");
                }
                Log.Info($"Задача {TaskId} : Задачи завершена");
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
