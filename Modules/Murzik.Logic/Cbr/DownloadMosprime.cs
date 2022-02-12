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
    public class DownloadMosprime : Algorithm
    {
        private ICbrDownloader _cbrDownloader;
        private IXmlSaver _xmlSaver;
        public const string RunDateTime = "RunDateTime";
        public DownloadMosprime(ILogger logger, 
            ITaskActions taskAction,
            ICbrDownloader cbrDownloader,
            IXmlSaver xmlSaver) : base(logger, taskAction)
        {
            _cbrDownloader = cbrDownloader;
            _xmlSaver = xmlSaver;
        }

        public override TaskTypes TaskTypes => TaskTypes.DownloadMosPrimeCbrf;

        public override async Task Run()
        {
            await base.Run();

            try
            {
                Log.Info($"Задача {TaskId} : Задача загрузки mosprime из ЦБ начата");
                IsContinue();
                DateTime dateTime = _paramDescriptors.ConvertDate(RunDateTime);
                Log.Info($"Задача {TaskId} : загрузка mosprime за дату {dateTime.ToShortDateString()}");
                var xmlDocument = await _cbrDownloader.DownloadMosPrime(dateTime);
                Log.Info($"Задача {TaskId} : загрузка mosprime завершена");

                IsContinue();
                var saverJson = TaskAction.GetSaverJson(TaskId);
                if (saverJson is not null)
                {
                    Log.Info($"Задача {TaskId} : сохранение значений");
                    _xmlSaver.Deserialize(saverJson).Save(xmlDocument);
                    Log.Info($"Задача {TaskId} : значения сохранены");
                }
                Log.Info($"Задача {TaskId} : Задача загрузки mosprime из ЦБ закончена");
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
            {
                return _paramDescriptors;
            }
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
    }
}
