using Core.Cards;
using Core.Castle;
using Mirror;
using System;

namespace Core.Server
{
    /// <summary>
    /// Entity player
    /// </summary>
    public class PlayerData
    {
        public Guid Id { get; set; }
        public bool IsGuest { get; set; }
        public string Name { get; set; }       
        public NetworkConnectionToClient Connection { get; set; }       
        public Match CurrentMatch { get; set; }       
        public PlayerCards Cards { get; set; }        
        public CastleEntity Castle { get; set; }        
    }
}