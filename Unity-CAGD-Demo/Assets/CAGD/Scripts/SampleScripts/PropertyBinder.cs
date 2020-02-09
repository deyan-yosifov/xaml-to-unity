using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public abstract class PropertyBinder : MonoBehaviour
{
    [SerializeField]
    private string propertyName;
    private INotifyPropertyChanged viewModel;
    private PropertyInfo property;

    protected abstract void GetPropertyValue(object value);

    protected void SetPropertyValue(object value)
    {
        this.property.SetValue(this.viewModel, value);
    }

    private void Awake()
    {
        this.viewModel = this.GetComponentInParent<INotifyPropertyChanged>();
        this.viewModel.PropertyChanged += this.ViewModelPropertyChanged;
        Type viewModelType = this.viewModel.GetType();
        this.property = viewModelType.GetProperty(this.propertyName);
        this.GetPropertyValue();
    }

    private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.propertyName)
        {
            this.GetPropertyValue();
        }
    }

    private void GetPropertyValue()
    {
        object value = this.property.GetValue(this.viewModel);
        this.GetPropertyValue(value);
    }
}
