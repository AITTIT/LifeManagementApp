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
            builder.Services.AddSingleton<TodoStore>(); // ONE list for the whole app
            builder.Services.AddTransient<AddTaskViewModel>();
            builder.Services.AddTransient<AddTaskPage>();
            builder.Services.AddTransient<TodoListViewModel>();
            builder.Services.AddTransient<TodoListPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
