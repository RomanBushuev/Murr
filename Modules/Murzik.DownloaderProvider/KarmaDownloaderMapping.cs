using Dapper.FluentMap;
using Murzik.DownloaderProvider.DbEntities.Mappings;

namespace Murzik.DownloaderProvider
{
    public class KarmaDownloaderMapping
    {
        private static KarmaDownloaderMapping _instance;
        private bool _isInitialized = false;

        private KarmaDownloaderMapping()
        {

        }

        private static KarmaDownloaderMapping GetInstance()
        {
            if (_instance == null)
                _instance = new KarmaDownloaderMapping();
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
