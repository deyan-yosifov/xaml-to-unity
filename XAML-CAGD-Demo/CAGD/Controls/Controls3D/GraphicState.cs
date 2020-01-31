using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAGD.Controls.Controls3D
{
    internal class GraphicState : PreservableState<GraphicProperties>, INotifyPropertiesChanged
    {
        private GraphicProperties topProperties;
        private readonly Stack<HashSet<string>> propertyChanges;

        public GraphicState()
        {
            topProperties = this.Value;
            this.propertyChanges = new Stack<HashSet<string>>();
            this.propertyChanges.Push(new HashSet<string>());
            this.topProperties.PropertiesChanged += this.GraphicPropertiesChanged;
        }

        protected override void OnStatePreservedOverride()
        {
            base.OnStatePreservedOverride();
            this.DetachAttachToProperties();
            this.propertyChanges.Push(new HashSet<string>());
        }

        protected override void OnStateRestoredOverride()
        {
            base.OnStateRestoredOverride();
            this.DetachAttachToProperties();

            HashSet<string> properties = this.propertyChanges.Pop();
            this.OnPropertiesChanged(properties);
        }

        private void DetachAttachToProperties()
        {
            this.topProperties.PropertiesChanged -= this.GraphicPropertiesChanged;
            this.topProperties = this.Value;
            this.topProperties.PropertiesChanged += this.GraphicPropertiesChanged;
        }

        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        private void GraphicPropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            HashSet<string> properties = this.propertyChanges.Peek();

            foreach(string property in e.PropertyNames)
            {
                properties.Add(property);
            }

            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged(sender, e);
            }
        }

        private void OnPropertiesChanged(IEnumerable<string> propertyNames)
        {
            if (this.PropertiesChanged != null)
            {
                this.PropertiesChanged(this, new PropertiesChangedEventArgs(propertyNames));
            }
        }
    }
}
