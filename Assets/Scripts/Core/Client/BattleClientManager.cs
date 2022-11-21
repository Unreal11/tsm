using Core.Contracts;
using Core.Server;
using UnityEngine;
using Core.Castle;
using Core.Cards;
using System.Linq;
using System;
using Effects;
using UnityEngine.SceneManagement;
using Mirror;

namespace Core.Client
{
    public class BattleClientManager : MonoBehaviour
    {
        public static BattleClientManager instance;

        private PlayerData _myPlayerData;
        private PlayerData _enemyPlayerData;
        private bool _isMyTurn;
        private bool _canPlay;
        private bool _matchEnded;
        private float _timer;
        private float _elapsedTime;
        private int _numberTurnForFatigue;
        private int _damageFatigue;
        private int _numberTurn;
        private int _fatigueLimit;

        public static PlayerData GetMyData() => instance._myPlayerData;
        public static PlayerData GetEnemyData() => instance._enemyPlayerData;
        public static float GetTimeLeft() => instance._timer - instance._elapsedTime;
        public static bool IsMyTurn() => instance._isMyTurn;
        public static bool IsCanPlay() => instance._canPlay;
        public static bool IsMatchEnded() => instance._matchEnded;

        public static void ResetBattleClient()
        {
            instance._myPlayerData = new PlayerData
            {
                Name = "Unknow",
                Castle = new CastleEntity()
            };

            instance._enemyPlayerData = new PlayerData
            {
                Name = "Unknow",
                Castle = new CastleEntity()
            };

            instance._isMyTurn = false;
            instance._canPlay = false;
            instance._matchEnded = false;
            instance._timer = 0;
            instance._elapsedTime = 0;
            instance._numberTurnForFatigue = 0;
            instance._damageFatigue = 0;
            instance._numberTurn = 0;
            instance._fatigueLimit = 0;
        }

        public static void ResetTimer()
        {
            instance._elapsedTime = 0;
        }

        public static void SetTurn(bool isMyTurn)
        {
            if (CardObject.IsDiscardMode)
            {
                var cardObject = FindObjectsOfType<CardObject>()
                    .FirstOrDefault(c => c.card.NonDiscard == false);

                if (cardObject != null)
                    cardObject.Discard();

                SetCanPlay(false);

                CardObject.IsDiscardMode = false;
            }

            if (SceneManager.GetActiveScene().name == "Battle")
            {
                BattleUI.HideTipsWindow();

                if (instance._numberTurn % 2 == 0)
                {
                    if (instance._numberTurn == instance._numberTurnForFatigue - 2)
                        BattleUI.ActivateFatigueDamageText();

                    if (instance._numberTurn == instance._numberTurnForFatigue)
                        EffectSpawner.StartFatigueEffect();

                    if (instance._numberTurn >= instance._numberTurnForFatigue)
                    {
                        if (GetMyData().Castle.Wall.Health > 0)
                            BattleUI.DamageMyWall(instance._damageFatigue);
                        else
                            BattleUI.DamageMyTower(instance._damageFatigue);

                        if (GetEnemyData().Castle.Wall.Health > 0)
                            BattleUI.DamageEnemyWall(instance._damageFatigue);
                        else
                            BattleUI.DamageEnemyTower(instance._damageFatigue);

                        EffectSpawner.ApplyFatigueEffect();

                        if (instance._damageFatigue < instance._fatigueLimit)
                            instance._damageFatigue++;
                    }
                }

                BattleUI.SetTextFatigueDamage(instance._damageFatigue);
            }

            if (instance._matchEnded || SceneManager.GetActiveScene().name != "Battle")
                return;

            if (!instance.CheckEndMatch())
            {
                instance._isMyTurn = isMyTurn;

                ResetTimer();
                SetCanPlay(isMyTurn);

                if (!isMyTurn && CardObject.IsDiscardMode)
                {
                    var cardObjects = FindObjectsOfType<CardObject>()
                        .Where(c => c.GetCardPosition() != null
                            && !c.card.NonDiscard)
                        .ToList();

                    cardObjects.FirstOrDefault()!.OnEndDrag(null);
                }

                if (instance._numberTurn % 2 == 0)
                {
                    foreach (Resource resource in instance._myPlayerData.Castle.Resources)
                        BattleUI.AddMyResourceValue(resource.Name, resource.Income);

                    foreach (Resource resource in instance._enemyPlayerData.Castle.Resources)
                        BattleUI.AddEnemyResourceValue(resource.Name, resource.Income);
                }

                if (isMyTurn)
                {
                    BattleUI.SetTextMyTurn();
                    BattleUI.instanse.skipTurnButton.SetActive(true);
                }
                else
                {
                    BattleUI.SetTextEnemyTurn();
                    BattleUI.instanse.skipTurnButton.SetActive(false);
                }
            }

            instance._numberTurn++;
        }

        public static void SetCanPlay(bool canPlay)
        {
            instance._canPlay = canPlay;
        }

        public static void SetPlayerDatas(PlayerData myPlayerData, PlayerData enemyPlayerData)
        {
            instance._myPlayerData = myPlayerData;
            instance._enemyPlayerData = enemyPlayerData;
        }

        public bool CheckEndMatch()
        {
            try
            {
                if (_enemyPlayerData.Castle.Tower.Health <= 0
                   && _myPlayerData.Castle.Tower.Health <= 0)
                {
                    BattleUI.instanse.ShowDrawWindow();
                    _matchEnded = true;

                    return true;
                }

                if (_myPlayerData.Castle.Tower.Health <= 0
                    || _enemyPlayerData.Castle.Tower.Health >= _enemyPlayerData.Castle.Tower.MaxHealth)
                {
                    BattleUI.instanse.ShowLoseWindow();
                    _matchEnded = true;

                    return true;
                }

                if (_enemyPlayerData.Castle.Tower.Health <= 0
                    || _myPlayerData.Castle.Tower.Health >= _myPlayerData.Castle.Tower.MaxHealth)
                {
                    BattleUI.instanse.ShowWinWindow();
                    _matchEnded = true;

                    return true;
                }
            }
            catch (Exception e)
            {
                //TODO: Логгер сюда засунуть
                Debug.Log(e);
            }

            return false;
        }

        private void Start()
        {
            instance = this;

            NetworkClient.RegisterHandler<RequestBattleInfo>(requestBattleInfo =>
            {
                ResetBattleClient();

                PlayerData myData = new PlayerData
                {
                    Name = requestBattleInfo.YourName,
                    Castle = new CastleEntity()
                };

                PlayerData enemyData = new PlayerData
                {
                    Name = requestBattleInfo.EnemyName,
                    Castle = new CastleEntity()
                };

                SetPlayerDatas(myData, enemyData);

                BattleUI.instanse.myNickname.text = requestBattleInfo.YourName;
                BattleUI.instanse.enemyNickname.text = requestBattleInfo.EnemyName;
                BattleUI.instanse.textHealthMyTower.text = requestBattleInfo.YourTowerHealth.ToString();
                BattleUI.instanse.textHealthEnemyTower.text = requestBattleInfo.EnemyTowerHealth.ToString();
                BattleUI.instanse.textHealthMyWall.text = requestBattleInfo.YourWallHealth.ToString();
                BattleUI.instanse.textHealthEnemyWall.text = requestBattleInfo.EnemyWallHealth.ToString();
                BattleUI.instanse.enemyResourceIncome_1.text = enemyData.Castle.GetResource("Resource_1").Income.ToString();
                BattleUI.instanse.enemyResourceIncome_2.text = enemyData.Castle.GetResource("Resource_2").Income.ToString();
                BattleUI.instanse.enemyResourceIncome_3.text = enemyData.Castle.GetResource("Resource_3").Income.ToString();
                BattleUI.instanse.myResourceIncome_1.text = myData.Castle.GetResource("Resource_1").Income.ToString();
                BattleUI.instanse.myResourceIncome_2.text = myData.Castle.GetResource("Resource_2").Income.ToString();
                BattleUI.instanse.myResourceIncome_3.text = myData.Castle.GetResource("Resource_3").Income.ToString();
                BattleUI.instanse.enemyResourceValue_1.text = enemyData.Castle.GetResource("Resource_1").Value.ToString();
                BattleUI.instanse.enemyResourceValue_2.text = enemyData.Castle.GetResource("Resource_2").Value.ToString();
                BattleUI.instanse.enemyResourceValue_3.text = enemyData.Castle.GetResource("Resource_3").Value.ToString();
                BattleUI.instanse.myResourceValue_1.text = myData.Castle.GetResource("Resource_1").Value.ToString();
                BattleUI.instanse.myResourceValue_2.text = myData.Castle.GetResource("Resource_2").Value.ToString();
                BattleUI.instanse.myResourceValue_3.text = myData.Castle.GetResource("Resource_3").Value.ToString();

                _timer = requestBattleInfo.Timer;
                _numberTurnForFatigue = requestBattleInfo.TurnFatigue;
                _damageFatigue = requestBattleInfo.StartDamageFatigue;
                _fatigueLimit = requestBattleInfo.FatigueLimit;

                _numberTurn = 0;
                _matchEnded = false;

                BattleUI.HideWaitStartWindow();
                SetTurn(requestBattleInfo.IsYourTurn);
            }, false);
        }

        private void FixedUpdate()
        {
            _elapsedTime += Time.fixedDeltaTime;

            if (_elapsedTime > _timer)
                _elapsedTime = _timer;
        }
    }
}