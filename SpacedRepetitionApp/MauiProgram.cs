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
            builder.Services.AddSingleton<ReviewService>();

            // FIX: StudyViewModel e StudyPage eram Singleton — o ViewModel nunca reiniciava
            // os cards entre sessões. Transient garante nova instância a cada navegação.
            builder.Services.AddTransient<StudyViewModel>();
            builder.Services.AddTransient<StudyPage>();

            // FIX: CreateCardPage não estava registrada no DI — era instanciada
            // manualmente via MauiContext dentro da View (má prática).
            builder.Services.AddTransient<CreateCardPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
