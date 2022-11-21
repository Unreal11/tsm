using Core.Cards;
using Core.Contracts;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Client
{
    public class CardClientController : MonoBehaviour
    {
        private static CardClientController instance;

        public static void SendRequestCardAction(RequestCardDto requestCardDto)
        {
            NetworkClient.Send(requestCardDto);
        }

        private void Start()
        {
            instance = this;

            NetworkClient.RegisterHandler<RequestCardDto>(HandleCardAction, false);
        }

        private void HandleCardAction(RequestCardDto requestCardDto)
        {
            if (SceneManager.GetActiveScene().name != "Battle")
                return;

            switch (requestCardDto.ActionType)
            {
                case CardActionType.YouPlayed:
                    YouPlayedCard(requestCardDto.CardId);
                    break;
                case CardActionType.EnemyPlayed:
                    EnemyPlayedCard(requestCardDto.CardId);
                    break;
                case CardActionType.Draft:
                    DraftCard(requestCardDto.CardId);
                    break;
            }
        }

        private void YouPlayedCard(Guid cardId)
        {
            Debug.Log($"You played card: {cardId}");
        }

        private void EnemyPlayedCard(Guid cardId)
        {
            Debug.Log($"Enemy played card: {cardId}");

            CardSpawner.SpawnEnemyCard(cardId);
        }

        private void DraftCard(Guid cardId)
        {
            Debug.Log($"Draft card: {cardId}");

            CardSpawner.SpawnDraftCard(cardId);
        }
    }
}