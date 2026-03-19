using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp;

public partial class MainPage : ContentPage
{
    private readonly HomeViewModel _vm;
    private readonly IServiceProvider _services;

    public MainPage(HomeViewModel vm, IServiceProvider services)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm      = vm;
        _services = services;
    }

    // Revisão normal — só cards vencidos
    private async void OnStudyTodayClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<StudyPage>();
        page.DefinirModo(StudyMode.Today);
        await Navigation.PushAsync(page);
    }

    // Estudar tudo — todos os cards independente do intervalo
    private async void OnStudyAllClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<StudyPage>();
        page.DefinirModo(StudyMode.All);
        await Navigation.PushAsync(page);
    }

    private async void OnAddCardClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(_services.GetRequiredService<CreateCardPage>());

    private async void OnStatsClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(_services.GetRequiredService<StatsPage>());

    // Swipe direita → editar
    private async void OnEditSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.BindingContext is Card card)
        {
            var page = _services.GetRequiredService<EditCardPage>();
            page.CarregarCard(card);
            await Navigation.PushAsync(page);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCards();
    }
}
