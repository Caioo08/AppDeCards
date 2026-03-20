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
                fonts.AddFont("OpenSans-Regular.ttf",  "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Serviços ───────────────────────────────────────────────────
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<CardService>();
        builder.Services.AddSingleton<ReviewService>();
        builder.Services.AddSingleton<StatsService>();

        // Ponto 3: StudySessionService é Transient (estado por sessão)
        builder.Services.AddTransient<StudySessionService>();

        // ── ViewModels ─────────────────────────────────────────────────
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<StatsViewModel>();
        builder.Services.AddTransient<StudyViewModel>();
        builder.Services.AddTransient<EditCardViewModel>();

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
