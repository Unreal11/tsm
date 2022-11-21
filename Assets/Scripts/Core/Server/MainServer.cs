using Core.Logging;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Server
{
    /// <summary>
    /// Main server component
    /// </summary>
    [DisallowMultipleComponent]
    public class MainServer : MonoBehaviour
    {
        private static MainServer instance;

        private List<PlayerData> _authPlayers;
        private List<NetworkConnectionToClient> _connectedPlayers;

        private IGameLogger _gameLogger;

        /// <summary>
        /// Add an unauthorized player to the server
        /// </summary>
        /// <param name="player">Connected player</param>
        public static void AddConnectedPlayer(NetworkConnectionToClient player)
        {
            if (!instance._connectedPlayers.Contains(player))
            {
                instance._connectedPlayers.Add(player);

                instance._gameLogger.Log($"{player.address} connected to server!", LogTypeMessage.Info);
                DebugManager.AddLineDebugText($"Connected count: {instance._connectedPlayers.Count}", "ConnectCount");
            }
        }

        /// <summary>
        /// Add an authorized player to the server
        /// </summary>
        /// <param name="player">Authorized player</param>
        public static void AddAuthPlayer(PlayerData player)
        {
            if (!instance._authPlayers.Contains(player))
            {
                instance._authPlayers.Add(player);

                instance._gameLogger.Log($"{player.Name} ({player.Id}) auth to server!", LogTypeMessage.Info);
            }
        }

        /// <summary>
        /// Remove an unauthorized player from the server
        /// </summary>
        /// <param name="player">Disconnected player</param>
        public static void RemoveConnectedPlayer(NetworkConnectionToClient player)
        {
            instance._connectedPlayers.Remove(player);

            instance._gameLogger.Log($"{player.address} disconnected!", LogTypeMessage.Info);
            DebugManager.AddLineDebugText($"Connected count: {instance._connectedPlayers.Count}", "ConnectCount");
        }

        /// <summary>
        /// Remove an authorized player from the server
        /// </summary>
        /// <param name="player">Remove authorized player</param>
        public static void RemoveAuthPlayer(PlayerData player)
        {
            instance._authPlayers.Remove(player);

            instance._gameLogger.Log($"{player.Name} ({player.Id}) logout!", LogTypeMessage.Info);
        }

        /// <summary>
        /// Get player data by id
        /// </summary>
        /// <param name="id">Player id</param>
        public static PlayerData GetPlayerData(Guid id)
        {
            return instance._authPlayers.FirstOrDefault(p => p.Id == id);
        }

        private void Awake()
        {
            instance = this;

            _authPlayers = new List<PlayerData>();
            _connectedPlayers = new List<NetworkConnectionToClient>();
            _gameLogger = new ConsoleLogger(new List<LogTypeMessage>
            {
                LogTypeMessage.Info,
                LogTypeMessage.Warning,
                LogTypeMessage.Error
            });
        }

        private void Start()
        {
            TowerSmashNetwork.ServerOnConnectEvent.AddListener(AddConnectedPlayer);
            TowerSmashNetwork.ServerOnDisconnectEvent.AddListener(RemoveConnectedPlayer);

            TowerSmashNetwork.ServerOnStart.AddListener(() =>
            {
                DebugManager.AddLineDebugText($"Server running!", "ServerRunning");

                instance._gameLogger.Log($"Server running!", LogTypeMessage.Info);
            });
            TowerSmashNetwork.ServerOnStop.AddListener(() =>
            {
                DebugManager.AddLineDebugText($"Server stop!", "ServerRunning");
                instance._gameLogger.Log($"Server stoped!", LogTypeMessage.Info);
            });

            TowerSmashNetwork.ServerRun();
        }
    }
}