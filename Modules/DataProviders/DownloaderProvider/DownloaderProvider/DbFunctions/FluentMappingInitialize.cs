using Dapper.FluentMap;
using DownloaderProvider.DatabaseEntities.Mappings;

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
                    config.AddMap(new DbCalculationJsonMap());
                    config.AddMap(new DbKarmaDownloadJobMap());
                    config.AddMap(new DbKarmaServiceMap());
                    config.AddMap(new DbSaverJsonMap());
                    config.AddMap(new DbDataSourceMap());
                });
                instance._isInitialized = true;
            }
        }
    }
}
