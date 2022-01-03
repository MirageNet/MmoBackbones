using LUD.DataStructures;
using LUD.Logging;
using Mirage;
using Mirage.Serialization;

namespace LUD.Core
{
    public class MasterServerMessageHandler : MessageHandler
    {
        public readonly Dictionary<ServerDataInfo, HashSet<INetworkPlayer>> RegionServers = new(new ServerDataInfoComparer());

        protected override void InvokeHandler(INetworkPlayer player, int msgType, NetworkReader reader, ArraySegment<byte> packet)
        {
            if (messageHandlers.TryGetValue(msgType, out NetworkMessageDelegate msgDelegate))
            {
                msgDelegate.Invoke(player, reader);
            }
            else
            {
                ServerDataInfo serverId = RegionServers.FirstOrDefault(x => x.Value.TryGetValue(player, out _)).Key;

                if(RegionServers.TryGetValue(serverId, out HashSet<INetworkPlayer>? servers))
                {
                    foreach (INetworkPlayer server in servers)
                    {
                        if (server == player) continue;

                        server.Send(packet);

                        MessagePacker.MessageTypes.TryGetValue(msgType, out Type type);

                        LogFactory.Log($"[MessageHandler] - Passing message: {type.Name} along to server: {server.Connection}", LogType.Log);
                    }
                }
                else
                {
                    RegionServers.Remove(serverId);

                    logger.Log(MessagePacker.MessageTypes.TryGetValue(msgType, out Type type)
                        ? $"Unexpected message {type} received from {player}. Did you register a handler for it?"
                        : $"Unexpected message ID {msgType} received from {player}. May be due to no existing RegisterHandler for this message.");
                }
            }
        }

        public MasterServerMessageHandler(IObjectLocator objectLocator, bool disconnectOnException) : base(objectLocator, disconnectOnException)
        {
        }
    }
}
