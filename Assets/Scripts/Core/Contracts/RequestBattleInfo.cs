using Mirror;
using System;

namespace Core.Contracts
{
    public struct RequestBattleInfo : NetworkMessage
    {
        public Guid AccountId { get; set; }

        public string YourName { get; set; }
        public string EnemyName { get; set; }

        public int Timer { get; set; }
        public int TurnFatigue { get; set; }
        public int StartDamageFatigue { get; set; }
        public int FatigueLimit { get; set; }

        public int YourTowerHealth { get; set; }
        public int EnemyTowerHealth { get; set; }

        public int YourWallHealth { get; set; }
        public int EnemyWallHealth { get; set; }

        public bool IsYourTurn { get; set; }
    }
}