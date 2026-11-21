using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LifeManagementApp.Services;
using LifeManagementApp.ViewModels;
using LifeManagementApp.Views;
using LifeManagementApp.Data;

namespace LifeManagementApp;
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

        // --- database (part 1) ---
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "todos.db");
        Debug.WriteLine($"Database: {dbPath}");
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
            .LogTo(message => Debug.WriteLine(message), LogLevel.Information)); // NEW: SQL to output

        builder.Services.AddSingleton<ITodoRepository, EfTodoRepository>();
        builder.Services.AddSingleton<TodoStore>();
        builder.Services.AddSingleton<IPreferences>(Preferences.Default);
        builder.Services.AddSingleton<ThemeService>();

        // Typed client: whoever asks for IWeatherService gets an OpenMeteoWeatherService with an
        // HttpClient set up like this. The base address and the timeout live only here.
        // A typed client is transient, so the classes that use it are transient too.
        builder.Services.AddHttpClient<IWeatherService, OpenMeteoWeatherService>(client =>
        {
            client.BaseAddress = new Uri("https://api.open-meteo.com/");   // ends with "/"
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        builder.Services.AddTransient<AddTaskViewModel>();
        builder.Services.AddTransient<AddTaskPage>();
        builder.Services.AddTransient<TodoListViewModel>();
        builder.Services.AddTransient<TodoListPage>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Create the database file and the tables if they do not exist yet.
        using (AppDbContext db = app.Services
                .GetRequiredService<IDbContextFactory<AppDbContext>>()
                .CreateDbContext())
        {
            db.Database.EnsureCreated();
        }

        return app;
    }
}
