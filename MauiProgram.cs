using Microsoft.Extensions.Logging;
using LifeManagementApp.Services;
using LifeManagementApp.ViewModels;
using LifeManagementApp.Views;

namespace LifeManagementApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<ITodoRepository, JsonTodoRepository>(); // NEW: the only line that says "JSON"
            builder.Services.AddSingleton<TodoStore>();
            builder.Services.AddSingleton<IPreferences>(Preferences.Default);
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddTransient<AddTaskViewModel>();
            builder.Services.AddTransient<AddTaskPage>();
            builder.Services.AddTransient<TodoListViewModel>();
            builder.Services.AddTransient<TodoListPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
