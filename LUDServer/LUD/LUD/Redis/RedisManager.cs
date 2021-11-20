using System.Diagnostics;
using LUD.Logging;
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
        protected internal readonly ISubscriber RedisSubscriber;

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
            LogFactory.Log("[Redis Manager] Booting up.", LogType.Log);

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            var config = new ConfigurationOptions
            {
                Password = _configuration?.GetSection("Redis:Password").Value,
                EndPoints = { _configuration?.GetSection("Redis:Connection").Value }
            };

            try
            {
                LogFactory.Log("[Redis Manager] Connecting to redis server.", LogType.Log);

                RedisConnection = ConnectionMultiplexer.Connect(config);

                _mainDatabase = RedisConnection.GetDatabase(_databaseNumber);

                LogFactory.Log($"[Redis Manager] Grabbing database: {_databaseNumber}", LogType.Log);

                RedisSubscriber = RedisConnection.GetSubscriber();
            }
            catch (Exception e)
            {
                LogFactory.Log(e.Message, LogType.Exception);

                throw new RedisConnectionException(ConnectionFailureType.AuthenticationFailure, e.Message);
            }
        }
    }
}
