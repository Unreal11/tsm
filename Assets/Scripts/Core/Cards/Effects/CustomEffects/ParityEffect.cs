using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Parity Effect", order = 52)]
    public class ParityEffect : Effect
    {
        public string nameResource;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            Resource usedPlayerResource = usedPlayer.Castle.GetResource(nameResource);
            Resource enemyPlayerResource = enemyPlayer.Castle.GetResource(nameResource);

            if (usedPlayerResource.Income > enemyPlayerResource.Income) 
                enemyPlayerResource.AddIncome(usedPlayerResource.Income - enemyPlayerResource.Income);
            else
                usedPlayerResource.AddIncome(enemyPlayerResource.Income - usedPlayerResource.Income);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            Resource myPlayerResource = BattleClientManager.GetMyData().Castle.GetResource(nameResource);
            Resource enemyPlayerResource = BattleClientManager.GetEnemyData().Castle.GetResource(nameResource);

            if (myPlayerResource.Income > enemyPlayerResource.Income)
                BattleUI.AddEnemyResourceIncome(nameResource, myPlayerResource.Income - enemyPlayerResource.Income);
            else
                BattleUI.AddMyResourceIncome(nameResource, enemyPlayerResource.Income - myPlayerResource.Income);

            yield return null;
        }
    }
}