using Core.Castle;
using Core.Client;
using Core.Server;
using Effects;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Damage Wall Effect", order = 51)]
    public class DamageWallEffect : Effect
    {
        [Min(0)]
        public int damage;
        public bool isEnemyDamage;
        public GameObject EffectAnimation;
        
        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            CastleEntity castle = isEnemyDamage ? enemyPlayer.Castle : usedPlayer.Castle;

            castle.Wall.Damage(damage);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isEnemyDamage)
            {
                if (isSender)
                {
                    BattleUI.DamageEnemyWall(damage);

                    if (BattleClientManager.GetEnemyData().Castle.Wall.Health > 0)
                    {
                        EffectSpawner.SpawnEffect(
                            EffectSpawner.instance.effectDamageFlying,
                            GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                            BattleUI.instanse.enemyWall.transform.position);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.DamageMyWall(damage);

                    if (BattleClientManager.GetMyData().Castle.Wall.Health > 0)
                    {
                        EffectSpawner.SpawnEffect(
                            EffectSpawner.instance.effectDamageFlying,
                            GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                            BattleUI.instanse.myWall.transform.position);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                if (isSender)
                {
                    BattleUI.DamageMyWall(damage);

                    if (BattleClientManager.GetMyData().Castle.Wall.Health > 0)
                    {
                        EffectSpawner.SpawnEffect(
                            EffectSpawner.instance.effectDamageFlying,
                            GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                            BattleUI.instanse.myWall.transform.position);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.DamageEnemyWall(damage);

                    if (BattleClientManager.GetEnemyData().Castle.Wall.Health > 0)
                    {
                        EffectSpawner.SpawnEffect(
                            EffectSpawner.instance.effectDamageFlying,
                            GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                            BattleUI.instanse.enemyWall.transform.position);
                    }

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
}