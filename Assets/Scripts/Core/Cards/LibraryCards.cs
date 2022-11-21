using Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Cards
{
    public class LibraryCards : MonoBehaviour
    {
        private static LibraryCards instance;

        public List<CardData> CardDatas;

        private bool isCustomDeck;
        private List<CardData> customDeckDatas;
        private bool isShuffle;

        private void Start()
        {
            try
            {
                instance = this;

                var customCardNames = Configurator.data["CustomDecks"]["customDecks"]
                    .Replace(" ", string.Empty)
                    .Split('|')
                    .ToList();

                isCustomDeck = bool.Parse(Configurator.data["CustomDecks"]["isCustomDeck"]);
                isShuffle = bool.Parse(Configurator.data["CustomDecks"]["doShuffle"]);
                customDeckDatas = new List<CardData>();               
            
                foreach (var cardName in customCardNames)
                {
                    var card = CardDatas.FirstOrDefault(c => c.Name == cardName);
                
                    if (card != null)
                        customDeckDatas.Add(card);
                }
            }
            catch (Exception e)
            {
            }           
        }

        public static CardData GetCard(Guid id)
        {
            return instance.CardDatas.FirstOrDefault(c => c.Id == id.ToString());
        }

        public static List<CardData> GetPlayerCards(PlayerData playerData)
        {
            List<CardData> playerCards = new List<CardData>();

            if (instance.isCustomDeck)
            {
                instance.customDeckDatas.ForEach(c => playerCards.Add(c));
            }
            else
            {
                instance.CardDatas.ForEach(c => playerCards.Add(c));
            }

            if (instance.isShuffle)
            {
                playerCards.Shuffle();
            }
            
            return playerCards;
        }
    }
}