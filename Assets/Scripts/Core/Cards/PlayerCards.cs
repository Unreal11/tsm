using Core.Contracts;
using Core.Logging;
using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Core.Cards
{
    public class PlayerCards
    {
        public List<Guid> CardsIdDeck { get; private set; }
        public List<Guid> CardsIdHand { get; private set; }

        private int _maxCardsInHand;
        private NetworkConnectionToClient _connection;
        private IGameLogger _gameLogger;

        public PlayerCards(List<Guid> cards, NetworkConnectionToClient connection)
        {
            CardsIdDeck = cards;
            CardsIdHand = new List<Guid>();

            _connection = connection;
            _maxCardsInHand = 6;

            _gameLogger = new ConsoleLogger(new List<LogTypeMessage>
            {
                 LogTypeMessage.Error,
                 LogTypeMessage.Warning,
                 LogTypeMessage.Info,
                 LogTypeMessage.Low
            });
        }

        public void RemoveCardFromHand(Guid cardId)
        {
            CardsIdHand.Remove(cardId);
        }

        public IEnumerator FillHand()
        {
            for (int i = 0; i < _maxCardsInHand; i++)
            {
                CardsIdHand.Add(GetAndTakeNearestCard());

                yield return new WaitForSeconds(0.6f);
            }      
        }

        public Guid GetAndTakeNearestCard() 
        {
            Guid id = CardsIdDeck.LastOrDefault();

            if (id == null)
                _gameLogger.Log($"Card take error! Connection: [{_connection}]", LogTypeMessage.Warning);

            CardsIdDeck.Remove(CardsIdDeck.LastOrDefault());
            CardsIdHand.Add(id);
            _connection.Send(new RequestCardDto
            {
                ActionType = CardActionType.Draft,
                CardId = id
            });

            return id;
        }

        public void ShuffleCard(Guid cardId, int maxIndex = 0)
        {
            if (maxIndex > CardsIdDeck.Count - 1)
                maxIndex = CardsIdDeck.Count - 1;

            System.Random random = new System.Random();

            CardsIdDeck.Insert(random.Next(0, maxIndex), cardId);

            _gameLogger.Log($"[{_connection}] Count cards in deck: {CardsIdDeck.Count}", LogTypeMessage.Low);
        }
    }
}