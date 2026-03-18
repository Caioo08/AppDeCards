using Microsoft.Extensions.Logging;
using SpacedRepetitionApp.Services;
using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp
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

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<CardService>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<StudyViewModel>();
            builder.Services.AddSingleton<StudyPage>();
            builder.Services.AddSingleton<ReviewService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
