using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Cards
{
    public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static bool isPlayable { get; private set; }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            isPlayable = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPlayable = false;
        }
    }
}