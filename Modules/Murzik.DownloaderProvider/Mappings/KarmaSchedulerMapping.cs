using Dapper.FluentMap;
using Murzik.DownloaderProvider.DbEntities.Mappings;

namespace Murzik.DownloaderProvider
{
    public class KarmaSchedulerMapping
    {
        private static KarmaSchedulerMapping _instance;
        private bool _isInitialized = false;

        private KarmaSchedulerMapping()
        {

        }

        private static KarmaSchedulerMapping GetInstance()
        {
            if (_instance == null)
                _instance = new KarmaSchedulerMapping();
            return _instance;
        }

        public static void Initialize()
        {
            var instance = GetInstance();
            if (!instance._isInitialized)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new DbProcedureTaskMap());
                    config.AddMap(new DbProcedureInfoMap());
                });
                instance._isInitialized = true;
            }
        }
    }
}
