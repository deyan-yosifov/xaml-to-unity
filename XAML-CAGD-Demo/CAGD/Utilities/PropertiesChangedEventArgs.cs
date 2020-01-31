using System;
using System.Collections.Generic;

namespace CAGD.Utilities
{
    public class PropertiesChangedEventArgs : EventArgs
    {
        public PropertiesChangedEventArgs(IEnumerable<string> propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public IEnumerable<string> PropertyNames
        {
            get;
            private set;
        }
    }
}
