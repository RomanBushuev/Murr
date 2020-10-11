using Dapper.FluentMap;
using DownloaderProvider.DatabaseEntities.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderProvider.DbFunctions
{
    public class FluentMappingInitialize
    {
        private static FluentMappingInitialize _instance;
        private bool _isInitialized = false;

        private FluentMappingInitialize()
        {

        }

        private static FluentMappingInitialize GetInstance()
        {
            if (_instance == null)
                _instance = new FluentMappingInitialize();
            return _instance;
        }

        public static void Initialize()
        {
            var instance = GetInstance();
            if (!instance._isInitialized)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new DbKarmaDownloadJobMap());
                });
                instance._isInitialized = true;
            }
        }
    }
}
