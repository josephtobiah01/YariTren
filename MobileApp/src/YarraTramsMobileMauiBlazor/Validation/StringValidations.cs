using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Helpers;

namespace YarraTramsMobileMauiBlazor.Validation
{
    class AlphanumericValidation : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {

            var _content = value as string;

            if (string.IsNullOrEmpty(_content))
                return true;

            return _content.IsAlphanumeric();
        }
    }

    class EmailValidationRule : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {

            var _content = value as string;

            if (string.IsNullOrEmpty(_content))
                return true;

            return _content.IsValidEmail();
        }
    }

    class ValidNameValidationRule : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            var _content = value as string;
            
            if (string.IsNullOrEmpty(_content))
                return true;

            return _content.IsValidName();
        }
    }

    class TextLimitValidationRule : IValidationRule<string>, IValidationTextLimit
    {
        public string ValidationMessage { get; set; }
        public int MaxChar { get; set; } = 1000;

        public bool Check(string value)
        {
            bool ret = true;

            if (!string.IsNullOrEmpty(value))
            {
                MaxChar = MaxChar <= 0 ? 1000 : MaxChar;
                ret = value.Length <= MaxChar;
            }

            ValidationMessage = string.IsNullOrEmpty(ValidationMessage) ? string.Format($"Input maximum {0}", MaxChar) : ValidationMessage;

            return ret;
        }
    }

    class PinLimitValidationRule : IValidationRule<string>, IValidationTextLimit
    {
        public string ValidationMessage { get; set; }
        public int MaxChar { get; set; } = 4;

        public bool Check(string value)
        {
            bool ret = true;

            if (!string.IsNullOrEmpty(value))
            {
                MaxChar = MaxChar <= 0 ? 4 : MaxChar;
                ret = value.Length <= MaxChar;
            }

            ValidationMessage = string.IsNullOrEmpty(ValidationMessage) ? string.Format($"Input maximum {0}", MaxChar) : ValidationMessage;

            return ret;
        }
    }

    class PhoneNumberValidation : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {

            var _content = value as string;
            _content = _content?.Replace("+", "");
            if (string.IsNullOrEmpty(_content))
                return true;

            //Regex digitsOnly = new Regex(@"[^\d]");
            //var _cleanContent = digitsOnly.Replace(_content, "");

            //return _cleanContent.Length >= 11 && _cleanContent.Length <= 13;
            return _content.IsValidPhoneNumber();
        }
    }

    class EmptyOrNullValidationRule : IValidationRule<string>
    {
        public string ValidationMessage { get; set; }

        public bool Check(string value)
        {
            if (value == null)
                return false;

            if (value.GetType() != typeof(string))
                return true;

            var _content = value as string;
            return !string.IsNullOrWhiteSpace(_content);
        }
    }
}
