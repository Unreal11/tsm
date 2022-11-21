using Mirror;
using System;

namespace Core.Contracts
{
    public struct RequestCardDto : NetworkMessage
    {
        public Guid AccountId { get; set; }
        public Guid CardId { get; set; }
        public CardActionType ActionType { get; set; }
    }
}
