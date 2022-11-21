using Mirror;
using System;

namespace Core.Contracts
{
    public struct AccountDto : NetworkMessage
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public bool IsGuest { get; set; }
    }
}