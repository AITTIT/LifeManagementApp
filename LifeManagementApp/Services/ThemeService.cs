namespace LifeManagementApp.Services;

public class ThemeService
{
    public AppTheme ToAppTheme(string theme)
    {
        return theme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified   // "System": follow the operating system
        };
    }

    public void Apply(string theme)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = ToAppTheme(theme);
        }
    }
}