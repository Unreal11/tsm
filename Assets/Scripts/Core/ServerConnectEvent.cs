using Mirror;
using UnityEngine.Events;

namespace Core
{
    /// <summary>
    /// Server event to handling connections
    /// </summary>
    public class ServerConnectEvent : UnityEvent<NetworkConnectionToClient>
    {
    }
}