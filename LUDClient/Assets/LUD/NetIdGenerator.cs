using System.Runtime.InteropServices;
using LUD.Authenticators;
using Mirage;
using UnityEngine;

namespace LUD
{
    public class NetIdGenerator : MonoBehaviour, INetworkIdentityGenerator
    {
        public ShardServerAuthenticator ShardServerAuthenticator;
        private NetworkServer _networkServer;

        #region Unity Methods

        private void Awake()
        {
            _networkServer = GetComponent<NetworkServer>();

            if (ShardServerAuthenticator == null)
                Debug.LogError("Please assign ShardServerAuthenticator in inspector.");

            if (_networkServer == null)
                Debug.LogError(
                    "Could not find network server. Please attach this script to where a network server component is.");
        }

        #endregion

        #region Overrides of ServerIdGenerator

        /// <summary>
        ///     Generates a new netid based on server sharding. This generator currently can only handle up
        ///     to 65k different ids. It will error out after that.
        /// </summary>
        /// <returns></returns>
        public uint GenerateNetId()
        {
            int netId = (ushort)_networkServer.World.SpawnedIdentities.Count;

            netId++;

            byte serverId = ShardServerAuthenticator.ShardId;

            uint test = (uint)(netId << 8 | serverId);

            return test;
        }

        public void UnPackShardId(uint packId, out uint netId, out byte serverId)
        {
            netId = packId >> 8;
            serverId = (byte)packId;
        }

        #endregion
    }
}
