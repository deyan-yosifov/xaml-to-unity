using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAGD.SampleScripts
{
    [RequireComponent(typeof(Slider))]
    public class SliderPropertyBinder : PropertyBinder
    {
        private Slider slider;

        protected override void Awake()
        {
            this.slider = this.GetComponent<Slider>();
            this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));

            base.Awake();
        }

        protected override void GetPropertyValue(object value)
        {
            float v;

            if (this.slider.wholeNumbers)
            {
                v = (int)value;
            }
            else
            {
                v = (float)value;
            }
             
            this.slider.value = v;
        }

        private void OnSliderValueChanged(float value)
        {
            object v;

            if (slider.wholeNumbers)
            {
                v = (int)value;
            }
            else
            {
                v = value;
            }

            this.SetPropertyValue(v);
        }
    }
}
