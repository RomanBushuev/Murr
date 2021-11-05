using Murzik.Entities;
using Murzik.Entities.Enumerations;
using Murzik.Interfaces;
using Murzik.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Murzik.Logic
{
    public abstract class Algorithm : IAlgorithm
    {
        public long TaskId { get; set; }
        protected List<ParamDescriptor> _paramDescriptors = null;
        protected CancellationTokenSource TaskTokenSource = new();
        protected CancellationToken TaskToken = new(false);
        protected CancellationTokenSource IsAliveTokenSource;
        protected CancellationToken IsAliveToken;
        protected ILogger Log;
        protected ITaskActions TaskAction;

        public Algorithm(ILogger logger, ITaskActions taskAction)
        {
            Log = logger;
            TaskAction = taskAction;
        }

        protected virtual void BeforeRun()
        {
            TaskTokenSource = new CancellationTokenSource();
            TaskToken = TaskTokenSource.Token;

            IsAliveTokenSource = new CancellationTokenSource();
            IsAliveToken = IsAliveTokenSource.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    Log.Info($"Опрашиваем задачу {TaskId}");
                    var isAlive = TaskAction.IsAlive(TaskId);
                    if(!isAlive)
                    {
                        TaskTokenSource.Cancel();
                        IsAliveTokenSource.Cancel();
                    }

                    await Task.Delay(10000);
                    if(IsAliveToken.IsCancellationRequested)
                    {
                        Log.Info($"Закончили опрашивать задачу {TaskId}");
                        IsAliveToken.ThrowIfCancellationRequested();
                    }
                }
            });
        }

        public virtual async Task Run()
        {
            BeforeRun();
        }

        public virtual IReadOnlyCollection<ParamDescriptor> GetParamDescriptors()
        {
            return _paramDescriptors;
        }

        public abstract TaskTypes TaskTypes { get; }

        public virtual void SetParamDescriptors(ParamDescriptor paramDescriptor)
        {
            if (_paramDescriptors == null)
            {
                GetParamDescriptors();
            }
            var needParam = _paramDescriptors.FirstOrDefault(z => z.Ident == paramDescriptor.Ident);
            needParam = ParamDescriptorExtensions.ConvertParam(needParam, paramDescriptor);
        }

        public virtual CalculationJson Serialize()
        {
            throw new ArgumentException("");
        }

        public void IsContinue()
        {
            if (TaskToken.IsCancellationRequested)
                TaskToken.ThrowIfCancellationRequested();
        }
    }
}
