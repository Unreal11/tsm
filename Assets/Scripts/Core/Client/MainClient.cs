using Core.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Client
{
    /// <summary>
    /// Main client component
    /// </summary>
    [DisallowMultipleComponent]
    public class MainClient : MonoBehaviour
    {
        private static MainClient instance;

        private Guid _clientId;
        private bool _isConnected;
        private bool _isAuth;
        private IGameLogger _gameLogger;

        public static Guid GetClientId() => instance._clientId;
        public static bool IsConnected() => instance._isConnected;
        public static bool IsAuth() => instance._isAuth;

        public static void SetClientId(Guid ClientId)
        {
            instance._clientId = ClientId;
        }

        public static void SetAuth(bool isAuth)
        {
            instance._isAuth = isAuth;
        }

        private void Start()
        {
            instance = this;

            _gameLogger = new ConsoleLogger(new List<LogTypeMessage>
            {
                LogTypeMessage.Info,
                LogTypeMessage.Warning,
                LogTypeMessage.Error
            });

            TowerSmashNetwork.ClientOnConnectEvent.AddListener(() => 
            {
                _isConnected = true;
                instance._gameLogger.Log($"Connected!", LogTypeMessage.Info);
                DebugManager.AddLineDebugText($"Connected! ", "ClientConnect");
            });
            TowerSmashNetwork.ClientOnDisconnectEvent.AddListener(() => 
            {
                _isConnected = false;
                instance._gameLogger.Log($"Disconnected!", LogTypeMessage.Info);
                DebugManager.AddLineDebugText($"Disconnected! ", "ClientConnect");
            });

            TowerSmashNetwork.ClientRun();

            instance._gameLogger.Log($"Client starting...", LogTypeMessage.Info);
            DebugManager.AddLineDebugText($"Connecting... ", "ClientConnect");
        }
    }
}