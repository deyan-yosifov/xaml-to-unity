using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.Controls.PointerHandlers
{
    public class PointerHandlersController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isPointerEntered = false;

        private void Update()
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            
            if (this.isPointerEntered && wheel != 0)
            {
                Debug.Log($"PointerHandlersController mouse wheel {wheel}.");
            }
        }

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

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            this.isPointerEntered = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            this.isPointerEntered = false;
        }
    }
}
