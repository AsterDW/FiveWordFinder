using FiveWordFinderWpf.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FiveWordFinderWpf.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IActivatable, INotifyDataErrorInfo
    {
        private bool _isActive = false;
        public bool IsActive 
        { 
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
                OnPropertyChanged();
            }
        }

        private Dictionary<string, List<string>> _propertyErrors = new Dictionary<string, List<string>>();
        private Dictionary<string, List<Func<ValidationResult>>> _propertyValidators = new Dictionary<string, List<Func<ValidationResult>>>();
        protected Dictionary<string, List<string>> PropertyErrors { get { return _propertyErrors; } }

        public bool HasErrors
        {
            get { return PropertyErrors.Values.Sum(v => v.Count) > 0; }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            OnValidateProperty(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void OnIsActiveChanged()
        {
            
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName) &&
                PropertyErrors.TryGetValue(propertyName, out var errors))
            {
                return errors;
            }

            return Enumerable.Empty<ValidationResult>();
        }

        protected void RegisterPropertyValidator(string propertyName, Func<ValidationResult> validator)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                if (!_propertyValidators.TryGetValue(propertyName, out var validators))
                {
                    validators = new List<Func<ValidationResult>>();
                    _propertyValidators.Add(propertyName, validators);
                }
                validators.Add(validator);
            }
        }

        protected void OnValidateProperty([CallerMemberName]string propertyName = "")
        {
            if (_propertyValidators.TryGetValue(propertyName, out var validators))
            {
                _propertyErrors.Remove(propertyName);
                foreach (var v in validators)
                {
                    var r = v();
                    if (!r.IsValid)
                    {
                        AddPropertyError(propertyName, r.ErrorContent?.ToString() ?? string.Empty);
                    }
                }
                OnErrorsChanged(propertyName);
            }
        }

        private void AddPropertyError(string propertyName, string message)
        {
            if (!_propertyErrors.TryGetValue(propertyName, out var errors))
            {
                errors = new List<string>();
                _propertyErrors.Add(propertyName, errors);
            }
            errors.Add(message);
        }
    }
}
