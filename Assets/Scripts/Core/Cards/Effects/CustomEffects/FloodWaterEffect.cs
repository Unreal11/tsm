using Core.Client;
using Core.Server;
using System.Collections;
using System.Collections.Generic;
using Core.Castle;
using UnityEngine;

namespace Core.Cards.Effects.CustomEffects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Flood Water Effect", order = 52)]
    public class FloodWaterEffect : Effect
    {
        [Header("Effect if condition true")]
        public List<Effect> trueEffects;
        [Header("Effect if condition false")]
        public List<Effect> falseEffects;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            if (usedPlayer.Castle.Wall.Health < enemyPlayer.Castle.Wall.Health)
                trueEffects.ForEach(e => e.Execute(usedPlayer, enemyPlayer));
            else
                falseEffects.ForEach(e => e.Execute(enemyPlayer, usedPlayer));
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isSender)
            {
                if (BattleClientManager.GetMyData().Castle.Wall.Health < 
                    BattleClientManager.GetEnemyData().Castle.Wall.Health)
                {
                    foreach (var effect in trueEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(
                            cardObject, isSender));
                }
                else
                {
                    foreach (var effect in falseEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(
                            cardObject, isSender));
                }
            }
            else
            {
                if (BattleClientManager.GetEnemyData().Castle.Wall.Health <
                    BattleClientManager.GetMyData().Castle.Wall.Health)
                {
                    foreach (var effect in trueEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(
                            cardObject, isSender));
                }
                else
                {
                    foreach (var effect in falseEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(
                            cardObject, isSender));
                }
            }
            yield return null;
        }
    }
}
