using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Copping The Tech Effect", order = 52)]
    public class CoppingTheTechEffect : Effect
    {
        public string nameResource;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            Resource usedPlayerResource = usedPlayer.Castle.GetResource(nameResource);
            Resource enemyPlayerResource = enemyPlayer.Castle.GetResource(nameResource);

            if (usedPlayerResource.Income < enemyPlayerResource.Income)
                enemyPlayerResource.AddIncome(usedPlayerResource.Income - enemyPlayerResource.Income);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            Resource myPlayerResource = BattleClientManager.GetMyData().Castle.GetResource(nameResource);
            Resource enemyPlayerResource = BattleClientManager.GetEnemyData().Castle.GetResource(nameResource);

            if (isSender)
            {
                if (myPlayerResource.Income < enemyPlayerResource.Income)
                    BattleUI.AddMyResourceIncome(nameResource, enemyPlayerResource.Income - myPlayerResource.Income);
            }
            else
            {
                if (enemyPlayerResource.Income < myPlayerResource.Income)
                    BattleUI.AddEnemyResourceIncome(nameResource, myPlayerResource.Income - enemyPlayerResource.Income);
            }

            yield return null;
        }
    }
}