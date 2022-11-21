using Mirror;
using System;

namespace Core.Contracts
{
    public struct Pinger : NetworkMessage
    {
        public Guid AccountId { get; set; }
    }
}