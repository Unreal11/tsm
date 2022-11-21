using Core.Cards;
using Core.Castle;
using Core.Contracts;
using Core.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IniParser;
using IniParser.Model;
using UnityEngine;

namespace Core.Server
{
    /// <summary>
    /// Match entity
    /// Responsible for match logic
    /// </summary>
    public class Match
    {
        public Guid Id { get; private set; }
        public Dictionary<PlayerData, bool> Players { get; private set; }
        public PlayerData CurrentPlayerTurn { get; private set; }
        public int TurnTime { get; private set; }
        public int PrepareTime { get; private set; }

        private Coroutine turnCoroutine;
        private int _numberTurn;
        private int _numberTurnForFatigue;
        private int _damageFatigue;
        private IGameLogger _gameLogger;

        public Match(List<PlayerData> player, IGameLogger gameLogger)
        {
            Id = Guid.NewGuid();
            Players = new Dictionary<PlayerData, bool>();
            TurnTime = int.Parse(Configurator.data["BattleConfiguration"]["turnTime"]);
            PrepareTime = 5;
            _gameLogger = gameLogger;

            player.ForEach(p =>
            {
                p.CurrentMatch = this;
                p.Castle = new CastleEntity();
                p.Cards = new PlayerCards(LibraryCards.GetPlayerCards(p)
                        .Select(c => Guid.Parse(c.Id))
                        .ToList(), p.Connection);

                Players.Add(p, false);
            });

            CurrentPlayerTurn = Players.FirstOrDefault().Key;

            _gameLogger.Log($"Create match {Id}!", LogTypeMessage.Info);
        }

        public virtual void NextTurn()
        {
            try
            {
                CurrentPlayerTurn.Connection.Send(new RequestMatchDto
                {
                    RequestType = MatchRequestType.EndTurn
                });
                CurrentPlayerTurn = Players.Keys.FirstOrDefault(p => p != CurrentPlayerTurn);
                CurrentPlayerTurn.Connection.Send(new RequestMatchDto
                {
                    RequestType = MatchRequestType.StartTurn
                });

                if (_numberTurn >= _numberTurnForFatigue - 2 && _numberTurn % 2 == 1)
                {
                    foreach (PlayerData player in Players.Keys)
                    {
                        if (player.Castle.Wall.Health > 0)
                            player.Castle.Wall.Damage(_damageFatigue);
                        else
                            player.Castle.Tower.Damage(_damageFatigue);
                    }

                    if (_damageFatigue < int.Parse(Configurator.data["BattleConfiguration"]["fatigueLimit"]))
                        _damageFatigue++;
                }

                PlayerData playerWin = CheckPlayerWin();

                if (playerWin == null)
                {
                    if (turnCoroutine != null)
                        MatchServerController.instance.StopCoroutine(turnCoroutine);

                    if (_numberTurn % 2 == 0)
                    {
                        foreach (PlayerData player in Players.Keys)
                            foreach (Resource resource in player.Castle.Resources)
                                resource.AddResource(resource.Income);
                    }

                    turnCoroutine = MatchServerController.instance.StartCoroutine(TurnTimer());
                    _numberTurn++;

                    _gameLogger.Log($"Match [{Id}]: Player turn - [{CurrentPlayerTurn.Id}]!", LogTypeMessage.Low);
                }
                else
                {
                    bool isDraw = playerWin.Castle.Tower.Health <= 0;

                    if (isDraw)
                    {
                        _gameLogger.Log($"Match [{Id}] stop: Draw!", LogTypeMessage.Info);

                        playerWin.Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.DrawMatch
                        });

                        Players.Keys.FirstOrDefault(p => p != playerWin).Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.DrawMatch
                        });
                    }
                    else
                    {
                        _gameLogger.Log($"Match [{Id}] stop: Player win - [{playerWin.Id}]", LogTypeMessage.Info);

                        playerWin.Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.WinMatch
                        });

                        Players.Keys.FirstOrDefault(p => p != playerWin).Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.LoseMatch
                        });
                    }

                    Stop();
                }
            }
            catch (Exception e)
            {
                _gameLogger.Log($"Match [{Id}] stop: {e}", LogTypeMessage.Warning);

                Stop();
            }
        }

        public virtual void PlayCard(RequestCardDto requestCardDto)
        {
            PlayerData sendPlayer = Players.Keys.FirstOrDefault(p => p.Id == requestCardDto.AccountId);
            PlayerData targetPlayer = Players.Keys.FirstOrDefault(p => p.Id != requestCardDto.AccountId);
            CardData card = LibraryCards.GetCard(requestCardDto.CardId);

            if (sendPlayer != null && targetPlayer != null && card != null)
            {
                if (sendPlayer.Cards.CardsIdHand.Contains(requestCardDto.CardId))
                {
                    try
                    {
                        sendPlayer.Cards.RemoveCardFromHand(requestCardDto.CardId);
                        sendPlayer.Cards.ShuffleCard(requestCardDto.CardId);
                        sendPlayer.Cards.GetAndTakeNearestCard();
                        card.Effects.ForEach(e => e.Execute(sendPlayer, targetPlayer));

                        foreach (PlayerData player in Players.Keys)
                        {
                            player.Connection.Send(new RequestCardDto
                            {
                                AccountId = requestCardDto.AccountId,
                                CardId = requestCardDto.CardId,
                                ActionType = (player.Id == requestCardDto.AccountId)
                                    ? CardActionType.YouPlayed
                                    : CardActionType.EnemyPlayed
                            });
                        }

                        CheckEndMatch();
                    }
                    catch (Exception e)
                    {
                        _gameLogger.Log($"Match [{Id}] error: {e}", LogTypeMessage.Warning);
                    }
                }
                else
                {
                    _gameLogger.Log($"Match [{Id}] error play card: card [{card}] is not found in hand", LogTypeMessage.Warning);
                }
            }
            else
            {
                _gameLogger.Log($"Match [{Id}] error play card: card - [{card}]" +
                    $", send player - [{sendPlayer}]" +
                    $", target Player - [{targetPlayer}]", LogTypeMessage.Warning);
            }
        }

        public virtual void DiscardCard(RequestCardDto requestCardDto)
        {
            PlayerData sendPlayer = Players.Keys.FirstOrDefault(p => p.Id == requestCardDto.AccountId);
            CardData card = LibraryCards.GetCard(requestCardDto.CardId);

            if (sendPlayer != null && card != null)
            {
                if (sendPlayer.Cards.CardsIdHand.Contains(requestCardDto.CardId))
                {
                    sendPlayer.Cards.RemoveCardFromHand(requestCardDto.CardId);
                    sendPlayer.Cards.ShuffleCard(requestCardDto.CardId);
                    sendPlayer.Cards.GetAndTakeNearestCard();
                }
            }
        }

        public void CheckEndMatch()
        {
            PlayerData playerWin = CheckPlayerWin();

            if (playerWin != null)
            {
                _gameLogger.Log($"Match [{Id}] stop: Player win - [{playerWin.Id}]", LogTypeMessage.Info);

                try
                {
                    //playerWin.Connection.Send(new RequestMatchDto
                    //{
                    //    RequestType = MatchRequestType.EndTurn
                    //});

                    //Players.Keys.FirstOrDefault(p => p != playerWin).Connection.Send(new RequestMatchDto
                    //{
                    //    RequestType = MatchRequestType.EndTurn
                    //});

                    bool isDraw = playerWin.Castle.Tower.Health <= 0;

                    if (isDraw)
                    {
                        playerWin.Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.DrawMatch
                        });

                        Players.Keys.FirstOrDefault(p => p != playerWin).Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.DrawMatch
                        });
                    }
                    else
                    {
                        playerWin.Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.WinMatch
                        });

                        Players.Keys.FirstOrDefault(p => p != playerWin).Connection.Send(new RequestMatchDto
                        {
                            RequestType = MatchRequestType.LoseMatch
                        });
                    }
                }
                catch (Exception e)
                {
                    _gameLogger.Log($"Match [{Id}] error: {e}", LogTypeMessage.Warning);
                }
                finally
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Start this match
        /// </summary>
        public void Start()
        {
            _numberTurn = 0;
            _numberTurnForFatigue = int.Parse(Configurator.data["BattleConfiguration"]["fatigueTurnStart"]);
            _damageFatigue = int.Parse(Configurator.data["BattleConfiguration"]["fatigueDamageStart"]);

            foreach (PlayerData playerData in Players.Keys)
                MatchServerController.instance.StartCoroutine(playerData.Cards.FillHand());

            turnCoroutine = MatchServerController.instance.StartCoroutine(TurnTimer());

            _gameLogger.Log($"Match [{Id}] start!", LogTypeMessage.Info);
        }

        /// <summary>
        /// Stop this match
        /// </summary>
        public void Stop()
        {
            foreach (PlayerData playerData in Players.Keys)
            {
                if (playerData.Connection != null)
                {
                    playerData.Connection.Send(new RequestMatchDto
                    {
                        AccountId = playerData.Id,
                        RequestType = MatchRequestType.ExitMatch
                    });
                }

                playerData.CurrentMatch = null;
            }

            Players.Clear();

            if (turnCoroutine != null)
                MatchServerController.instance.StopCoroutine(turnCoroutine);

            turnCoroutine = null;

            _gameLogger.Log($"Match [{Id}] stop!", LogTypeMessage.Info);

            MatchServerController.RemoveMatch(Id);
        }

        /// <summary>
        /// Set player as ready
        /// </summary>
        public void PlayerReady(Guid playerId)
        {
            PlayerData readyPlayer = Players.Keys.FirstOrDefault(p => p.Id == playerId);

            if (readyPlayer != null)
            {
                Players[readyPlayer] = true;

                _gameLogger.Log($"{readyPlayer.Name} [{readyPlayer.Id}] ready for match [{Id}]!", LogTypeMessage.Low);

                CheckForStartMatch();
            }
            else
            {
                _gameLogger.Log($"{readyPlayer.Name} [{readyPlayer.Id}] not found in match [{Id}]!", LogTypeMessage.Warning);
            }
        }

        public void SetDebugInfo()
        {
            DebugManager.AddLineDebugText("T1: " + Players.Keys.First().Castle.Tower.Health.ToString(), "t1");
            DebugManager.AddLineDebugText("W1: " + Players.Keys.First().Castle.Wall.Health.ToString(), "w1");
            DebugManager.AddLineDebugText("T2: " + Players.Keys.Last().Castle.Tower.Health.ToString(), "t2");
            DebugManager.AddLineDebugText("W2: " + Players.Keys.Last().Castle.Wall.Health.ToString(), "w2");
            DebugManager.AddLineDebugText("Turn: " + _numberTurn, "t");
            DebugManager.AddLineDebugText("FatigueDamage: " + _damageFatigue, "fd");
        }

        private void CheckForStartMatch()
        {
            foreach (var player in Players)
                if (player.Value == false)
                    return;

            Start();
        }

        private PlayerData CheckPlayerWin()
        {
            foreach (var player in Players.Keys)
                if (player.Castle.Tower.Health <= 0)
                    return Players.Keys.FirstOrDefault(p => p != player);

            foreach (var player in Players.Keys)
                if (player.Castle.Tower.Health >= player.Castle.Tower.MaxHealth)
                    return Players.Keys.FirstOrDefault(p => p == player);

            return null;
        }

        private IEnumerator TurnTimer()
        {
            yield return new WaitForSeconds(TurnTime);

            NextTurn();
        }
    }
}