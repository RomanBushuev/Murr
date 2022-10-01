using Dapper.FluentMap;
using Murzik.ReaderMurrData.DbEntities.Mappings;

namespace Murzik.ReaderMurrData.Mappings
{
    internal class MurrDataMapping
    {
        private static MurrDataMapping _instance;
        private bool _isInitialized = false;

        private MurrDataMapping()
        {

        }

        private static MurrDataMapping GetInstance()
        {
            if (_instance == null)
                _instance = new MurrDataMapping();
            return _instance;
        }

        public static void Initialize()
        {
            var instance = GetInstance();
            if (!instance._isInitialized)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new DbFinDataSourcesMap());
                });
                instance._isInitialized = true;
            }
        }
    }
}

