using Microsoft.Extensions.Configuration;

namespace Murzik.Tests
{
    public static class TestExtensions
    {
        public static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return config;
        }
    }
}
