using Mirror;
using System;

namespace Core.Contracts
{
    public struct RequestMatchDto : NetworkMessage
    {
        public Guid AccountId { get; set; }
        public MatchRequestType RequestType { get; set; }
    }
}
