using Microsoft.Extensions.Logging;
using SpacedRepetitionApp.Services;
using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp;

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

        // ── Serviços (Singleton — stateless ou conexão única) ──────────
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<CardService>();
        builder.Services.AddSingleton<ReviewService>();
        builder.Services.AddSingleton<StatsService>();

        // ── ViewModels ─────────────────────────────────────────────────
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<StatsViewModel>();
        builder.Services.AddTransient<StudyViewModel>();   // reinicia a cada sessão
        builder.Services.AddTransient<EditCardViewModel>(); // nova instância por edição

        // ── Pages ──────────────────────────────────────────────────────
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<StatsPage>();
        builder.Services.AddTransient<StudyPage>();
        builder.Services.AddTransient<CreateCardPage>();
        builder.Services.AddTransient<EditCardPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
