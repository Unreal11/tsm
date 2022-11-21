using Core.Client;
using Core.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Cards.Effects.CustomEffects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Barracks Effect", order = 52)]
    public class BarracksEffect : Effect
    {
        public string nameResource;

        [Header("Effect if condition true")]
        public List<Effect> trueEffects;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            var usedPlayerResource = usedPlayer.Castle.GetResource(nameResource);
            var enemyPlayerResource = enemyPlayer.Castle.GetResource(nameResource);

            if (usedPlayerResource.Income < enemyPlayerResource.Income)
                trueEffects.ForEach(e => e.Execute(usedPlayer, enemyPlayer));
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            var myPlayerResource = BattleClientManager.GetMyData().Castle.GetResource(nameResource);
            var enemyPlayerResource = BattleClientManager.GetEnemyData().Castle.GetResource(nameResource);

            if (isSender)
            {
                if (myPlayerResource.Income < enemyPlayerResource.Income)
                {
                    foreach (Effect effect in trueEffects)
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
            }

            yield return null;
        }
    }
}