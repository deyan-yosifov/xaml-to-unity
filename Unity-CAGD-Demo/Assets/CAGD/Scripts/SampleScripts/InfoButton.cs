using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.SampleScripts
{
    public class InfoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Color32 normalColor = new Color32(0, 0, 255, 0x44);
        public Color32 hoverColor = new Color32(0, 0, 255, 0xaa);
        public Color32 pressedColor = new Color32(0, 0, 255, 0xff);
        public GameObject tooltip;
        private bool isPressed = false;
        private bool isHovered = false;
        private TextMeshProUGUI[] texts;

        private void Awake()
        {
            this.texts = this.GetComponentsInChildren<TextMeshProUGUI>();
            this.ChangeColor();
            this.UpdateTooltip();
        }

        private void UpdateTooltip()
        {
            this.tooltip.SetActive(this.isPressed);
        }

        private void ChangeColor()
        {
            Color32 color = this.isPressed ? this.pressedColor : (this.isHovered ? this.hoverColor : this.normalColor);

            for (int i = 0; i < this.texts.Length; i++)
            {
                TextMeshProUGUI text = this.texts[i];
                text.overrideColorTags = true;
                text.color = color;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            this.isPressed = true;
            this.ChangeColor();
            this.UpdateTooltip();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.isPressed = false;
            this.ChangeColor();
            this.UpdateTooltip();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            this.isHovered = true;
            this.ChangeColor();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            this.isHovered = false;
            this.ChangeColor();
        }
    }
}
