using DevExpress.Maui.Core;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Helpers;
using BindableBase = YarraTramsMobileMauiBlazor.Helpers.BindableBase;

namespace YarraTramsMobileMauiBlazor.Validation
{
    public class ValidatableObject<T> : BindableBase
    {
        private T value;

        public T Value
        {
            get { return value; }
            set { SetPropertyValue(ref this.value, value); }
        }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set
            {
                SetPropertyValue(ref _title, value);
                if (_initial)
                {
                    _initial = false;
                    _origTitle = Title;
                }
            }
        }

        private Keyboard _keyBoard = Keyboard.Default;
        public Keyboard KeyBoard
        {
            get { return _keyBoard; }
            set
            {
                SetPropertyValue(ref _keyBoard, value);
            }
        }

        private string _placeholder = "";
        public string Placeholder
        {
            get { return _placeholder; }
            set
            {
                SetPropertyValue(ref _placeholder, value);
            }
        }


        private string _error;

        public string Error
        {
            get { return _error; }
            set { SetPropertyValue(ref _error, value); }
        }


        private bool _isValid;
        public bool IsValid
        {

            get { return _isValid; }
            set { SetPropertyValue(ref _isValid, value); }
        }


        public ValidatableObject()
        {
            Rules = new List<IValidationRule<T>>();
            _isValid = true;
            _error = "";
        }


        string _origTitle = "";
        bool _initial = true;
        public List<IValidationRule<T>> Rules { get; set; }

        public void Validate()
        {


            Error = "";
            var _errors = Rules.Where(x => !x.Check(Value)).Select(x => x.ValidationMessage.Replace(".", "") + ".").ToList();


            int _count = 1;
            foreach (var x in _errors)
            {
                Error += string.Format("{0}{1}", x, _count >= _errors.Count ? "" : "\n");
                _count++;
            }


            IsValid = !_errors.Any();
            Title = IsValid ? _origTitle : Error;
        }

        public void ClearError()
        {
            Title = _origTitle;
            IsValid = true;
            Error = string.Empty;
        }

        public static implicit operator ValidatableObject<T>(ValidatableObject<DateTime> v)
        {
            throw new NotImplementedException();
        }
    }
}
