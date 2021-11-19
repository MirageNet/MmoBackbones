using LUD.Logging;
using Mirage;
using Mirage.Serialization;
namespace LUD.Core
{
    public class MasterServerMessageHandler : MessageHandler
    {
        public readonly Dictionary<byte, HashSet<INetworkPlayer>> RegionServers = new();

        protected override void InvokeHandler(INetworkPlayer player, int msgType, NetworkReader reader, ArraySegment<byte> packet)
        {
            if (messageHandlers.TryGetValue(msgType, out NetworkMessageDelegate msgDelegate))
            {
                msgDelegate.Invoke(player, reader);
            }
            else
            {
                byte serverId = RegionServers.FirstOrDefault(x => x.Value.TryGetValue(player, out player)).Key;

                if(RegionServers.TryGetValue(serverId, out HashSet<INetworkPlayer>? servers))
                {
                    foreach (INetworkPlayer server in servers)
                    {
                        if (server == player) continue;

                        player.Send(packet);

                        LogFactory.Log($"Passing information along to server: {server.Connection}", LogType.Log);
                    }
                }
                else
                {
                    RegionServers.Remove(serverId);

                    try
                    {
                        Type type = MessagePacker.GetMessageType(msgType);
                        throw new InvalidDataException(
                            $"Unexpected message {type} received in {this}. Did you register a handler for it?");
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new InvalidDataException(
                            $"Unexpected message ID {msgType} received in {this}. May be due to no existing RegisterHandler for this message.");
                    }
                }
            }
        }

        public MasterServerMessageHandler(bool disconnectOnException) : base(disconnectOnException)
        {
        }
    }
}
