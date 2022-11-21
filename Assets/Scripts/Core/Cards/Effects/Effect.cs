using Core.Server;
using System.Collections;
using UnityEngine;

namespace Core.Cards.Effects
{
    public abstract class Effect : ScriptableObject
    {
        public abstract void Execute(PlayerData usedPlayer, PlayerData enemyPlayer);
        public abstract IEnumerator Animation(CardObject cardObject, bool isSender);
    }
}