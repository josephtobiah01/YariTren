using System.Windows.Input;

namespace YarraTramsMobileMauiBlazor.CustomControls;

public partial class ImageButton : ContentView
{
	public ImageButton()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ImageButton), string.Empty, propertyChanged: OnTextChanged);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(ImageButton), default(ImageSource), propertyChanged: OnImageSourceChanged);

    public static readonly BindableProperty ValueProperty =
       BindableProperty.Create(nameof(Value), typeof(bool), typeof(ImageButton), false, propertyChanged: OnValueChanged);

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ImageButton), null);

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public ImageSource ImageSource
    {
        get { return (ImageSource)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    public bool Value
    {
        get { return (bool)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ImageButton)bindable;
        control.text.Text = (string)newValue;
    }

    private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ImageButton)bindable;
        control.image.Source = (ImageSource)newValue;
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ImageButton)bindable;
        bool value = (bool)newValue;
        control.ToggleClick(value);
    }

    private void Handle_Tapped_Button(object sender, System.EventArgs e)
    {
        if (Command != null && Command.CanExecute(null))
        {
            Command.Execute(null);
        }
    }

    private void ToggleClick(bool val)
    {
        if (val)
        {
            value.Text = "yes";
            layout.BackgroundColor = Colors.Green;
            text.TextColor = Colors.White;
            //image.Source = "checked_icon.png";
            
        }
        else
        {
            layout.BackgroundColor = Colors.White;
            value.Text = "no";
            text.TextColor = Colors.Black;
            //image.Source = "unchecked_icon.png";

        }
    }

    
}