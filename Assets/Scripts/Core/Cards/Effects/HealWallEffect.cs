using Core.Castle;
using Core.Client;
using Core.Server;
using Effects;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Heal Wall Effect", order = 51)]
    public class HealWallEffect : Effect
    {
        [Min(0)]
        public int heal;
        public bool isSelfHeal;
        public GameObject EffectAnimation;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            CastleEntity castle = isSelfHeal ? usedPlayer.Castle : enemyPlayer.Castle;

            castle.Wall.Heal(heal);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isSelfHeal)
            {
                if (isSender)
                {
                    BattleUI.HealMyWall(heal);
                    EffectSpawner.SpawnEffect(
                        EffectSpawner.instance.effectHealFlying,
                        GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                        BattleUI.instanse.myWall.transform.position);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.HealEnemyWall(heal);
                    EffectSpawner.SpawnEffect(
                        EffectSpawner.instance.effectHealFlying,
                        GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                        BattleUI.instanse.enemyWall.transform.position);

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                if (isSender)
                {
                    BattleUI.HealEnemyWall(heal);
                    EffectSpawner.SpawnEffect(
                        EffectSpawner.instance.effectHealFlying,
                        GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                        BattleUI.instanse.enemyWall.transform.position);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.HealMyWall(heal);
                    EffectSpawner.SpawnEffect(
                        EffectSpawner.instance.effectHealFlying,
                        GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                        BattleUI.instanse.myWall.transform.position);

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
}