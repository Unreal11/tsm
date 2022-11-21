using Core.Castle;
using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Effect Add Income", order = 51)]
    public class AddIncomeEffect : Effect
    {
        public string nameResource;
        [Min(0)]
        public int income;
        public bool isSelfAdd;
        public GameObject EffectAnimation;

        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
            CastleEntity castle = isSelfAdd ? usedPlayer.Castle : enemyPlayer.Castle;

            castle.GetResource(nameResource).AddIncome(income);
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isSelfAdd)
            {
                if (isSender)
                {
                    BattleUI.AddMyResourceIncome(nameResource, income);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.AddEnemyResourceIncome(nameResource, income);

                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                if (isSender)
                {
                    BattleUI.AddEnemyResourceIncome(nameResource, income);

                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    BattleUI.AddMyResourceIncome(nameResource, income);

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }
}
