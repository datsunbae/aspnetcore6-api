using api_aspnetcore6.Helpers;
using StackExchange.Redis;

namespace api_aspnetcore6.Caching
{
    public class ConnectionHelper
    {
        static ConnectionHelper()
        {
            ConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var test = ConfigurationManagers.AppSetting["Caching:RedisURL"];
                return ConnectionMultiplexer.Connect(ConfigurationManagers.AppSetting["Caching:RedisURL"]);
            });
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}