using System;

namespace CAGD.Utilities
{
    public interface INotifyPropertiesChanged
    {
        event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;
    }
}
