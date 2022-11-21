using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;
using Effects;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Damage Effect", order = 51)]
    public class DamageEffect : Effect
    {
        [Min(0)]
        public int damage;
        public bool isEnemyDamage;
        public GameObject EffectAnimation;
        
        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            CastleEntity castle = isEnemyDamage ? enemyPlayer.Castle : usedPlayer.Castle;
            int damageCount = damage;
            int wallHealth = castle.Wall.Health;

            if (wallHealth >= damageCount)
            {
                castle.Wall.Damage(damageCount);
            }
            else
            {
                castle.Wall.Damage(damageCount);
                damageCount -= wallHealth;
                castle.Tower.Damage(damageCount);
            }
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isEnemyDamage)
            {
                if (isSender)
                {
                    int damageCount = damage;
                    int wallHealth = BattleClientManager.GetEnemyData().Castle.Wall.Health;

                    if (wallHealth >= damageCount)
                    {
                        BattleUI.DamageEnemyWall(damageCount);
                        ShowFlyEffect(BattleUI.instanse.enemyWall.transform.position);
                    }
                    else
                    {
                        damageCount -= wallHealth;

                        BattleUI.DamageEnemyWall(wallHealth);                        
                        BattleUI.DamageEnemyTower(damageCount);
                        ShowFlyEffect(BattleUI.instanse.enemyWall.transform.position);
                    }

                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    int damageCount = damage;
                    int wallHealth = BattleClientManager.GetMyData().Castle.Wall.Health;

                    if (wallHealth >= damageCount)
                    {
                        BattleUI.DamageMyWall(damageCount);
                        ShowFlyEffect(BattleUI.instanse.myWall.transform.position);
                    }
                    else
                    {
                        damageCount -= wallHealth;

                        BattleUI.DamageMyWall(wallHealth);                       
                        BattleUI.DamageMyTower(damageCount);
                        ShowFlyEffect(BattleUI.instanse.myWall.transform.position);
                    }

                    yield return new WaitForSeconds(2f);
                }
            }
            else
            {
                if (isSender)
                {
                    int damageCount = damage;
                    int wallHealth = BattleClientManager.GetMyData().Castle.Wall.Health;

                    if (wallHealth >= damageCount)
                    {
                        BattleUI.DamageMyWall(damageCount);
                        ShowFlyEffect(BattleUI.instanse.myWall.transform.position);
                    }
                    else
                    {
                        damageCount -= wallHealth;

                        BattleUI.DamageMyWall(wallHealth);                     
                        BattleUI.DamageMyTower(damageCount);
                        ShowFlyEffect(BattleUI.instanse.myWall.transform.position);
                    }

                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    int damageCount = damage;
                    int wallHealth = BattleClientManager.GetEnemyData().Castle.Wall.Health;

                    if (wallHealth >= damageCount)
                    {
                        BattleUI.DamageEnemyWall(damageCount);
                        ShowFlyEffect(BattleUI.instanse.enemyWall.transform.position);
                    }
                    else
                    {
                        damageCount -= wallHealth;

                        BattleUI.DamageEnemyWall(wallHealth);                        
                        BattleUI.DamageEnemyTower(damageCount);
                        ShowFlyEffect(BattleUI.instanse.enemyWall.transform.position);
                    }

                    yield return new WaitForSeconds(2f);
                }
            }
        }

        private void ShowFlyEffect(Vector3 target)
        {
            EffectSpawner.SpawnEffect(
                EffectSpawner.instance.effectDamageFlying,
                GameObject.Find("SpawnAnimationPlaceHolder").transform.position,
                target);
        }
    }
}