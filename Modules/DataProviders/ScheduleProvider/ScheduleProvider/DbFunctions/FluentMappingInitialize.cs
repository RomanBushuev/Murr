using Dapper.FluentMap;
using ScheduleProvider.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleProvider.DbFunctions
{
    public class FluentMappingInitialize
    {
        private static FluentMappingInitialize _instance;
        private bool _isInitialized = false;

        private FluentMappingInitialize()
        {

        }

        private static FluentMappingInitialize GetInitialize()
        {
            if (_instance == null)
                _instance = new FluentMappingInitialize();
            return _instance;
        }

        public static void Initialize()
        {
            var instance = GetInitialize();
            if(!instance._isInitialized)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new CbrForeignParamMap());
                    config.AddMap(new ProcedureTaskMap());
                    config.AddMap(new ProcedureInfoMap());
                });
                instance._isInitialized = true;
            }
        }
    }
}
