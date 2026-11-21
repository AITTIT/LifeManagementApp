using LifeManagementApp.Services;

namespace LifeManagementApp
{
    public partial class App : Application
    {
        // App is created by DI as well, so it can ask for services in its constructor.
        public App(IPreferences preferences, ThemeService themeService)
        {
            InitializeComponent();

            var value = preferences.Get(SettingsKeys.Theme, SettingsDefaults.Theme);
            UserAppTheme = themeService.ToAppTheme(value);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}