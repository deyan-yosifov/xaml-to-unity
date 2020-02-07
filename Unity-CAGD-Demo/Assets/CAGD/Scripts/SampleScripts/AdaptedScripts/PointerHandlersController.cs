using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.SampleScripts
{
    public class PointerHandlersController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Debug.Log("PointerHandlersController drag.");
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("PointerHandlersController down.");
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("PointerHandlersController up.");
        }
    }
}
