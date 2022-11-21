using Core.Client;
using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Effects/Create new Discard Effect", order = 51)]
    public class DiscardEffect : Effect
    {
        public override void Execute(PlayerData usedPlayer, PlayerData enemyPlayer)
        {
        }

        public override IEnumerator Animation(CardObject cardObject, bool isSender)
        {
            if (isSender)
            {
                CardObject.IsDiscardMode = true;

                BattleClientManager.SetCanPlay(true);
                BattleUI.ShowTipsWindow("Please, discard card with right click");

                while (CardObject.IsDiscardMode)
                    yield return null;
            }
        }
    }
}