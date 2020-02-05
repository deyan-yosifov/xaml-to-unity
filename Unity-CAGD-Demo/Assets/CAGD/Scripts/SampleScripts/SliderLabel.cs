using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CAGD.SampleScripts
{
    public class SliderLabel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private Slider slider;

        private void Awake()
        {
            this.UpdateValue();
        }

        public void UpdateValue()
        {
            this.text.text = this.slider.value.ToString();
        }
    }
}
