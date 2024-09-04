using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YarraTramsMobileMauiBlazor.CustomHandlers
{
    public sealed class CustomEntry : Entry
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CustomEntry), default(ICommand));

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(CustomEntry), 0);

        public static BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(CustomEntry), 0);

        public static BindableProperty PaddingProperty =
            BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(CustomEntry), new Thickness(5));

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CustomEntry), Colors.Transparent);

        public static BindableProperty LeftImageHeightProperty =
            BindableProperty.Create(nameof(LeftImageHeight), typeof(int), typeof(CustomEntry), 7);

        public static BindableProperty RightImageHeightProperty =
            BindableProperty.Create(nameof(RightImageHeight), typeof(int), typeof(CustomEntry), 7);

        public static BindableProperty LeftImageWidthProperty =
            BindableProperty.Create(nameof(LeftImageHeight), typeof(int), typeof(CustomEntry), 7);

        public static BindableProperty RightImageWidthProperty =
            BindableProperty.Create(nameof(RightImageHeight), typeof(int), typeof(CustomEntry), 7);

        public static readonly BindableProperty LeftImageProperty =
            BindableProperty.Create(nameof(LeftImage), typeof(string), typeof(CustomEntry), string.Empty);

        public static readonly BindableProperty RightImageProperty =
            BindableProperty.Create(nameof(RightImage), typeof(string), typeof(CustomEntry), string.Empty);

        public static readonly BindableProperty ImageAlignmentProperty =
            BindableProperty.Create(nameof(ImageAlignment), typeof(ImageAlignment), typeof(CustomEntry), ImageAlignment.Left);

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(CustomEntry), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;


        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public int LeftImageHeight
        {
            get => (int)GetValue(LeftImageHeightProperty);
            set => SetValue(LeftImageHeightProperty, value);
        }

        public int RightImageHeight
        {
            get => (int)GetValue(RightImageHeightProperty);
            set => SetValue(RightImageHeightProperty, value);
        }

        public int LeftImageWidth
        {
            get => (int)GetValue(LeftImageWidthProperty);
            set => SetValue(LeftImageHeightProperty, value);
        }

        public int RightImageWidth
        {
            get => (int)GetValue(RightImageWidthProperty);
            set => SetValue(RightImageHeightProperty, value);
        }

        public string LeftImage
        {
            get => (string)GetValue(LeftImageProperty);
            set => SetValue(LeftImageProperty, value);
        }

        public string RightImage
        {
            get => (string)GetValue(RightImageProperty);
            set => SetValue(RightImageProperty, value);
        }

        public ImageAlignment ImageAlignment
        {
            get => (ImageAlignment)GetValue(ImageAlignmentProperty);
            set => SetValue(ImageAlignmentProperty, value);
        }

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            set { base.SetValue(IsValidPropertyKey, value); }
        }
    }

    public enum ImageAlignment
    {
        Left,
        Right
    }
}
