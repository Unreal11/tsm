using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Mother Lode Effect", order = 52)]
    public class MotherLodeEffect : Effect
    {
        public string nameResource;

        [Header("Effect if condition true")]
        public List<Effect> trueEffects;
        [Header("Effect if condition false")]
        public List<Effect> falseEffects;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            Resource usedPlayerResource = usedPlayer.Castle.GetResource(nameResource);
            Resource enemyPlayerResource = enemyPlayer.Castle.GetResource(nameResource);

            if (usedPlayerResource.Income < enemyPlayerResource.Income)
                trueEffects.ForEach(e => e.Execute(usedPlayer, enemyPlayer));
            else
                falseEffects.ForEach(e => e.Execute(usedPlayer, enemyPlayer));
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            Resource myPlayerResource = BattleClientManager.GetMyData().Castle.GetResource(nameResource);
            Resource enemyPlayerResource = BattleClientManager.GetEnemyData().Castle.GetResource(nameResource);

            if (isSender)
            {
                if (myPlayerResource.Income < enemyPlayerResource.Income)
                {
                    foreach (Effect effect in trueEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(cardObject, isSender));
                }
                else
                {
                    foreach (Effect effect in falseEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(cardObject, isSender));
                }
            }
            else
            {
                if (myPlayerResource.Income > enemyPlayerResource.Income)
                {
                    foreach (Effect effect in trueEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(cardObject, isSender));
                }
                else
                {
                    foreach (Effect effect in falseEffects)
                        yield return cardObject.StartCoroutine(effect.Animation(cardObject, isSender));
                }
            }

            yield return null;
        }
    }
}