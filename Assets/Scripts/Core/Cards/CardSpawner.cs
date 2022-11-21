using System;
using UnityEngine;

namespace Core.Cards
{
    public class CardSpawner : MonoBehaviour
    {
        private static CardSpawner instance;

        [SerializeField]
        private CardObject card;
        [SerializeField]
        private Transform hand;
        [SerializeField]
        private Transform deckPosition;
        [SerializeField]
        private Transform enemyPosition;

        private void Awake()
        {
            instance = this;
        }

        public static void SpawnEnemyCard(Guid id)
        {
            CardObject cardObject = Instantiate(instance.card, 
                instance.enemyPosition.position, 
                instance.enemyPosition.rotation, 
                instance.hand);

            cardObject.card = LibraryCards.GetCard(id);
            cardObject.StartCoroutine(cardObject.CardEnemyPlayAnimation());
        }

        public static void SpawnDraftCard(Guid id)
        {
            CardObject cardObject = Instantiate(instance.card, 
                instance.deckPosition.position, 
                instance.deckPosition.rotation, 
                instance.hand);

            cardObject.card = LibraryCards.GetCard(id);
            cardObject.StartCoroutine(cardObject.CardDraftPlayAnimation());
        }
    }
}