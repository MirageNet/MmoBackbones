using System.Reflection;
using LUD.Authenticators;
using LUD.Logging;
using LUD.Messages;
using Mirage;
using Mirage.Sockets.Udp;
using UnityEngine;
using Microsoft.Extensions.Configuration;
using LogType = LUD.Logging.LogType;

namespace LUD.Core
{
    public class Server
    {
        #region Fields

        private readonly Dictionary<byte, Dictionary<byte, List<INetworkPlayer>>> _regionServers = new();
        private IConfiguration? _configuration;
        private NetworkServer? _server;

        #endregion

        #region Callback Listener's

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="player"></param>
        private void OnServerHasAuthenticated(AuthenticatorMessages.ServerAuthCode data, INetworkPlayer player)
        {
            if (!_regionServers.ContainsKey(data.ServerInfo.ServerId))
            {
                _regionServers.Add(data.ServerInfo.ServerId, new Dictionary<byte, List<INetworkPlayer>>());

                _regionServers[data.ServerInfo.ServerId]
                    .Add(data.ServerInfo.RegionId, new List<INetworkPlayer> { player });
            }
            else
            {
                _regionServers[data.ServerInfo.ServerId][data.ServerInfo.RegionId].Add(player);
            }
        }

        #endregion

        #region Class Specific

        /// <summary>
        ///     Create a new server.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        public Server(RegionAuthenticator authenticator)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator), "Authenticator passed in was null.");
            }

            Initialize(authenticator);
        }

        /// <summary>
        ///     Initializes a new server with configuration from appsettings.json file and run
        ///     internal mirage trigger to mimic unity firing on load.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        private void Initialize(RegionAuthenticator authenticator)
        {
            LogFactory.Log("[Master Server] Booting up.", LogType.Log);

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // This allows us to fire up mirage internal code to mimic unity system.
            RunInitializeMethods();

            CreateServer(authenticator);

            LogFactory.Log("[Master Server] Listening for new region servers.", LogType.Log);

            Update();
        }

        /// <summary>
        ///     Creates the necessary server and authenticators and starts up the server.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        private void CreateServer(RegionAuthenticator authenticator)
        {
            var socketFactory = new UdpSocketFactory
            {
                Port = Convert.ToUInt16(_configuration?.GetSection("Network:Port").Value)
            };

            authenticator.ServerHasAuthenticated += OnServerHasAuthenticated;

            _server = new NetworkServer { SocketFactory = socketFactory, authenticator = authenticator };

            _server.StartServer();
        }

        /// <summary>
        ///     Process server data.
        /// </summary>
        private async void Update()
        {
            while (true)
            {
                try
                {
                    _server?.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                await Task.Delay(5);
            }
        }

        /// <summary>
        ///     Initializes mirage to run.
        /// </summary>
        private void RunInitializeMethods()
        {
            var asm = Assembly.GetExecutingAssembly();

            MethodInfo[] methods = asm.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false).Length > 0)
                .ToArray();

            foreach (MethodInfo method in methods)
            {
                method.Invoke(null, null);
            }
        }

        /// <summary>
        ///     Shutdown server.
        /// </summary>
        public void Shutdown()
        {
            LogFactory.Log("[Master Server] Shutting down.", LogType.Log);
        }

        #endregion
    }
}
