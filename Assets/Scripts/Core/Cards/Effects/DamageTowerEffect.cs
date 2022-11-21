using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;
using Effects;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Damage Tower Effect", order = 51)]
    public class DamageTowerEffect : Effect
    {
        [Min(0)]
        public int damage;
        public bool isEnemyDamage;
        public GameObject EffectAnimation;
        
        private readonly Vector3 fireballTowerOffset = new Vector3(0f, 3f, 0f);
        
        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            CastleEntity castle = isEnemyDamage ? enemyPlayer.Castle : usedPlayer.Castle;

            castle.Tower.Damage(damage);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isEnemyDamage)
            {
                if (isSender)
                {
                    BattleUI.DamageEnemyTower(damage);
                    ShowFlyEffect(BattleUI.instanse.enemyTower.transform.position + fireballTowerOffset);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.DamageMyTower(damage);
                    ShowFlyEffect(BattleUI.instanse.myTower.transform.position + fireballTowerOffset);

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                if (isSender)
                {
                    BattleUI.DamageMyTower(damage);
                    ShowFlyEffect(BattleUI.instanse.myTower.transform.position + fireballTowerOffset);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.DamageEnemyTower(damage);
                    ShowFlyEffect(BattleUI.instanse.enemyTower.transform.position + fireballTowerOffset);

                    yield return new WaitForSeconds(0.2f);
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