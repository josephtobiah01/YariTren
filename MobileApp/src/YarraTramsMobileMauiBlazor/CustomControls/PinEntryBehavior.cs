using System.Text.RegularExpressions;
using YarraTramsMobileMauiBlazor.CustomHandlers;

namespace YarraTramsMobileMauiBlazor.CustomControls
{
    public class PinEntryBehavior : Behavior<CustomEntry>
    {
        public int MaxLength { get; set; } = 4;

        const string matchLenRegex = @"^(?!(.)\1{3})(?!19|20)(?!0123|1234|2345|3456|4567|5678|6789|7890|0987|9876|8765|7654|6543|5432|4321|3210)\d{4}$";
        protected override void OnAttachedTo(CustomEntry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(PinEntryBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnDetachingFrom(CustomEntry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as CustomEntry;
            ((CustomEntry)sender).IsValid = IsValid = (Regex.IsMatch(args.NewTextValue, matchLenRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((CustomEntry)sender).TextColor = IsValid ? Colors.Black : Colors.White;
        }
    }
}
