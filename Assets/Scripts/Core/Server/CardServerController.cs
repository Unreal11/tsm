using UnityEngine;
using Mirror;
using Core.Contracts;

namespace Core.Server
{
    public class CardServerController : MonoBehaviour
    {
        private static CardServerController instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            NetworkServer.RegisterHandler<RequestCardDto>(HandleCardAction, false);
        }

        private void HandleCardAction(NetworkConnectionToClient connection, RequestCardDto requestCardDto)
        {
            PlayerData player = MainServer.GetPlayerData(requestCardDto.AccountId);

            if (player != null)
            {
                switch (requestCardDto.ActionType) 
                {
                    case CardActionType.RequestPlay:
                        player.CurrentMatch?.PlayCard(requestCardDto);
                        break;
                    case CardActionType.RequestDiscard:
                        player.CurrentMatch?.DiscardCard(requestCardDto);
                        break;
                }
            }
        }
    }
}