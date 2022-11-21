using Core.Contracts;
using Core.Logging;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Server
{
    /// <summary>
    /// Match controller
    /// Responsible for the logic of finding a match among players
    /// </summary>
    [DisallowMultipleComponent]
    public class MatchServerController : MonoBehaviour
    {
        public static MatchServerController instance;

        /// <summary>
        /// Number of players in a match
        /// Default: 2
        /// </summary>
        public int MatchSize { get; set; } = 2;

        private List<PlayerData> _playersLookingForMatches;
        private List<Match> _matches;
        private IGameLogger _gameLogger;

        /// <summary>
        /// Add a player who is looking for a match
        /// </summary>
        /// <param name="player">Looking player</param>
        public static void RemoveMatch(Guid matchId)
        {
            Match match = instance._matches.FirstOrDefault(m => m.Id == matchId);

            instance._matches.Remove(match);
        }

        /// <summary>
        /// Add a player who is looking for a match
        /// </summary>
        /// <param name="player">Looking player</param>
        public static void AddPlayerLookingMatch(PlayerData player)
        {
            if (!instance._playersLookingForMatches.Contains(player))
            {
                var match = instance._matches
                  .Where(m => m.Players
                      .FirstOrDefault(p => p.Key.Id == player.Id).Key != null)
                  .FirstOrDefault();

                if (match != null)
                    match.Players.Remove(player);

                instance._playersLookingForMatches.Add(player);
                instance.CheckMatching();

                instance._gameLogger.Log($"[{player.Name} ({player.Id})] add looking for match!", LogTypeMessage.Low);
                DebugManager.AddLineDebugText($"Player searching match: {instance._playersLookingForMatches.Count}", "PlayersLookingForMatches");
            }
        }

        /// <summary>
        /// Remove a player who is looking for a match
        /// </summary>
        /// <param name="player">Looking player</param>
        public static void RemovePlayerLookingMatch(PlayerData player)
        {
            instance._playersLookingForMatches.Remove(player);

            instance._gameLogger.Log($"[{player.Name} ({player.Id})] remove looking for match!", LogTypeMessage.Low);
            DebugManager.AddLineDebugText($"Player searching match: {instance._playersLookingForMatches.Count}", "PlayersLookingForMatches");
        }

        private void Awake()
        {
            instance = this;

            _playersLookingForMatches = new List<PlayerData>();
            _matches = new List<Match>();
            _gameLogger = new ConsoleLogger(new List<LogTypeMessage>
            {
                LogTypeMessage.Info,
                LogTypeMessage.Warning,
                LogTypeMessage.Error,
                LogTypeMessage.Low
            });
        }

        private void Start()
        {
            TowerSmashNetwork.ServerOnDisconnectEvent.AddListener((serverConnect) =>
            {
                var match = _matches
                    .Where(m => m.Players
                        .FirstOrDefault(p => p.Key.Connection == serverConnect).Key != null)
                    .FirstOrDefault();

                if (match != null)
                {
                    foreach (PlayerData player in match.Players.Keys)
                    {
                        if (player != null && player.Connection != null)
                        {
                            player.Connection.Send(new RequestMatchDto
                            {
                                RequestType = MatchRequestType.WinMatch
                            });
                        }
                    }

                    match.Stop();
                }
            });

            NetworkServer.RegisterHandler<RequestBattleInfo>((connection, requestBattleDto) =>
            {
                var match = _matches
                    .Where(m => m.Players
                        .FirstOrDefault(p => p.Key.Id == requestBattleDto.AccountId).Key != null)
                    .FirstOrDefault();

                if (match != null)
                {
                    var yourData = match.Players.FirstOrDefault(p => p.Key.Id == requestBattleDto.AccountId).Key;
                    var enemyData = match.Players.FirstOrDefault(p => p.Key.Id != requestBattleDto.AccountId).Key;

                    connection.Send(new RequestBattleInfo
                    {
                        AccountId = requestBattleDto.AccountId,
                        YourName = yourData.Name,
                        EnemyName = enemyData.Name,
                        YourTowerHealth = yourData.Castle.Tower.Health,
                        EnemyTowerHealth = enemyData.Castle.Tower.Health,
                        YourWallHealth = yourData.Castle.Wall.Health,
                        EnemyWallHealth = enemyData.Castle.Wall.Health,
                        IsYourTurn = match.CurrentPlayerTurn.Id == requestBattleDto.AccountId,
                        Timer = match.TurnTime,
                        StartDamageFatigue = int.Parse(Configurator.data["BattleConfiguration"]["fatigueDamageStart"]),
                        TurnFatigue = int.Parse(Configurator.data["BattleConfiguration"]["fatigueTurnStart"]),
                        FatigueLimit = int.Parse(Configurator.data["BattleConfiguration"]["fatigueLimit"])
                    });

                    match.PlayerReady(requestBattleDto.AccountId);
                }
                else
                {
                    _gameLogger.Log($"Match, contains player {requestBattleDto.AccountId} not founded!", LogTypeMessage.Low);
                }
            }, false);

            NetworkServer.RegisterHandler<RequestMatchDto>((connection, requestMatchDto) =>
            {
                PlayerData playerData = MainServer.GetPlayerData(requestMatchDto.AccountId);

                if (playerData != null)
                {
                    switch (requestMatchDto.RequestType)
                    {
                        case MatchRequestType.FindingMatch:
                            AddPlayerLookingMatch(playerData);
                            break;
                        case MatchRequestType.CancelFindingMatch:
                            RemovePlayerLookingMatch(playerData);
                            break;
                        case MatchRequestType.ExitMatch:
                            LeaveMatch(playerData);
                            break;
                        case MatchRequestType.EndTurn:
                            RequestEndTurn(playerData);
                            break;
                    }
                }
                else
                {
                    _gameLogger.Log($"Player data is not found!", LogTypeMessage.Low);
                }
            }, false);
        }

        private void RequestEndTurn(PlayerData player)
        {
            var match = _matches
                    .Where(m => m.Players
                        .FirstOrDefault(p => p.Key.Id == player.Id).Key != null)
                    .FirstOrDefault();

            if (match != null)
                if (match.CurrentPlayerTurn == player)
                    match.NextTurn();
        }

        private void LeaveMatch(PlayerData player)
        {
            var match = _matches
                    .Where(m => m.Players
                        .FirstOrDefault(p => p.Key.Id == player.Id).Key != null)
                    .FirstOrDefault();

            if (match != null)
                player.Castle.Tower.Damage(player.Castle.Tower.Health);
        }

        private void CheckMatching()
        {
            if (instance._playersLookingForMatches.Count >= MatchSize)
            {
                List<PlayerData> players = instance._playersLookingForMatches.Take(MatchSize).ToList();

                _matches.Add(new Match(players, _gameLogger));
                players.ForEach(p =>
                {
                    RemovePlayerLookingMatch(p);
                    p.Connection.Send(new RequestMatchDto());
                });
            }
        }

        void Update()
        {
            if (_matches.Count > 0)
                _matches[0].SetDebugInfo();
        }
    }
}