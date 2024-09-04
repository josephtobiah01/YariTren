using YarraTramsMobileMauiBlazor.Validation;

namespace YarraTramsMobileMauiBlazor.CustomControls;

public partial class ValidateEntry : ContentView
{
	public ValidateEntry()
	{
		InitializeComponent();
	}

    ValidatableObject<string> _context = null;
    private void GetContext()
    {
        if (_context == null)
        {
            _context = (ValidatableObject<string>)this.BindingContext;
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        try
        {
            GetContext();
            _context.Validate();
        }
        catch { }
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        try
        {
            GetContext();
            _context.ClearError();
        }
        catch { }
    }
}