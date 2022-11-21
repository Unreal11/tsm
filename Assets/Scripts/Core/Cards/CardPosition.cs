using UnityEngine;

namespace Core.Cards
{
    public class CardPosition : MonoBehaviour
    {
        public GameObject card;
        public RectTransform rectTransform;

        public int startIndex;
        public int startPointerIndex;

        private void Start()
        {
            startPointerIndex = transform.GetSiblingIndex();
        }
    }
}