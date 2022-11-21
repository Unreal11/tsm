using Mirror;

namespace Core.Contracts
{
    public struct AuthDto : NetworkMessage
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsGuest { get; set; }
    }
}