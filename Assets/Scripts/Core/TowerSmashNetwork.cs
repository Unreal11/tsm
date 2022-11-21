using Mirror;

namespace Core
{
    /// <summary>
    /// Main network controller
    /// </summary>
    public class TowerSmashNetwork : NetworkManager
    {
        /// <summary>
        /// Server only
        /// Called when the server starts
        /// </summary>
        public static ServerEvent ServerOnStart { get; private set; }

        /// <summary>
        /// Server only
        /// Called when the server stopped
        /// </summary>
        public static ServerEvent ServerOnStop { get; private set; }

        /// <summary>
        /// Server only
        /// Called when the client connects to the server
        /// </summary>
        public static ServerConnectEvent ServerOnConnectEvent { get; private set; }

        /// <summary>
        /// Server only
        /// Called when the client disconnects from the server
        /// </summary>
        public static ServerConnectEvent ServerOnDisconnectEvent { get; private set; }

        /// <summary>
        /// Client only
        /// Called when the client connected to server
        /// </summary>
        public static ClientEvent ClientOnConnectEvent { get; private set; }

        /// <summary>
        /// Client only
        /// Called when the client disconnects from the server
        /// </summary>
        public static ClientEvent ClientOnDisconnectEvent { get; private set; }

        private static TowerSmashNetwork instance;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
            ServerOnStart = new ServerEvent();
            ServerOnStop = new ServerEvent();
            ServerOnConnectEvent = new ServerConnectEvent();
            ServerOnDisconnectEvent = new ServerConnectEvent();
            ClientOnConnectEvent = new ClientEvent();
            ClientOnDisconnectEvent = new ClientEvent();
        }

        protected override void OnStartServer()
        {
            base.OnStartServer();
            ServerOnStart.Invoke();
        }

        protected override void OnStopServer()
        {
            base.OnStopServer();
            ServerOnStop.Invoke();
        }

        protected override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientOnDisconnectEvent.Invoke();
        }

        protected override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientOnConnectEvent.Invoke();
        }

        protected override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            ServerOnConnectEvent.Invoke(conn);
        }

        protected override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            ServerOnDisconnectEvent.Invoke(conn);
        }

        /// <summary>
        /// Starts network as server
        /// </summary>
        public static void ServerRun()
        {
            instance.StartServer();
        }

        /// <summary>
        /// Starts network as client
        /// </summary>
        public static void ClientRun()
        {
            instance.StartClient();
        }
    }
}