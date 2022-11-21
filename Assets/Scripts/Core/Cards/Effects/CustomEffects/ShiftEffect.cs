using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Custom/Create new Shift Effect", order = 52)]
    public class ShiftEffect : Effect
    {
        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            int usedWallHealth = usedPlayer.Castle.Wall.Health;
            int enemyWallHealth = enemyPlayer.Castle.Wall.Health;

            usedPlayer.Castle.Wall.Health = enemyWallHealth;
            enemyPlayer.Castle.Wall.Health = usedWallHealth;
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            int usedWallHealth = BattleClientManager.GetMyData().Castle.Wall.Health;
            int enemyWallHealth = BattleClientManager.GetEnemyData().Castle.Wall.Health;

            if (usedWallHealth > enemyWallHealth)
            {
                BattleUI.DamageMyWall(usedWallHealth - enemyWallHealth);
                BattleUI.HealEnemyWall(usedWallHealth - enemyWallHealth);
            }
            else
            {
                BattleUI.HealMyWall(enemyWallHealth - usedWallHealth);
                BattleUI.DamageEnemyWall(enemyWallHealth - usedWallHealth);
            }

            yield return null;
        }
    }
}