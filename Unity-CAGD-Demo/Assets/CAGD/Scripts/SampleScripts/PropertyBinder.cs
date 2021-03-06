﻿using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace CAGD.SampleScripts
{
    public abstract class PropertyBinder : MonoBehaviour
    {
        [SerializeField]
        private string propertyName = null;
        private INotifyPropertyChanged viewModel;
        private PropertyInfo property;

        protected abstract void GetPropertyValue(object value);

        protected void SetPropertyValue(object value)
        {
            this.property.SetValue(this.viewModel, value);
        }

        protected virtual void Start()
        {
            this.viewModel = this.GetComponentInParent<INotifyPropertyChanged>();

            if (this.viewModel == null)
            {
                Debug.LogError($"PropertyBinder for {this.propertyName} cannot find view model in parent!");
            }
            else
            {
                Type viewModelType = this.viewModel.GetType();
                this.property = viewModelType.GetProperty(this.propertyName);

                if (this.property == null)
                {
                    Debug.LogError($"PropertyBinder cannot find property with name {this.propertyName}!");
                }
                else
                {
                    this.GetPropertyValue();
                    this.viewModel.PropertyChanged += this.ViewModelPropertyChanged;
                }
            }

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
}
