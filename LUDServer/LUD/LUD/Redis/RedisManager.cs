using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace LUD.Redis
{
    public class RedisManager
    {
        #region Fields

        protected readonly ConnectionMultiplexer? RedisConnection;
        private IConfiguration? _configuration;
        private int _databaseNumber;
        private IDatabase _mainDatabase;

        #endregion

        #region Properties

        /// <summary>
        ///     Are we connected to a redis server.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                Debug.Assert(RedisConnection != null, nameof(RedisConnection) + " != null");

                return RedisConnection.IsConnected;
            }
        }

        #endregion

        public RedisManager()
        {
            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var test = new ConfigurationOptions
            {
                Password = _configuration?.GetSection("Redis:Password").Value,
                EndPoints = { _configuration?.GetSection("Redis:Connection").Value }
            };

            RedisConnection = ConnectionMultiplexer.Connect(test);

            _mainDatabase = RedisConnection.GetDatabase(_databaseNumber);
        }
    }
}
