using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CAGD.SampleScripts
{
    [RequireComponent(typeof(Toggle))]
    public class TogglePropertyBinder : PropertyBinder
    {
        private Toggle toggle;

        protected override void Start()
        {
            this.toggle = this.GetComponent<Toggle>();
            this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));

            base.Start();
        }

        protected override void GetPropertyValue(object value)
        {
            this.toggle.isOn = (bool)value;
        }

        private void OnToggleValueChanged(bool value)
        {
            this.SetPropertyValue(value);
        }
    }
}
