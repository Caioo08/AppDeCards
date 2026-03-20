using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp;

public partial class MainPage : ContentPage
{
    private readonly HomeViewModel    _vm;
    private readonly IServiceProvider _services;

    public MainPage(HomeViewModel vm, IServiceProvider services)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm       = vm;
        _services = services;
        _vm.DeletarSolicitado += OnDeletarSolicitado;
    }

    private async void OnDeletarSolicitado(Card card)
    {
        var confirmar = await DisplayAlert(
            "Deletar card",
            $"Tem certeza que deseja deletar\n\"{card.Pergunta}\"?",
            "Deletar", "Cancelar");

        if (confirmar)
            _vm.ConfirmarDelete(card);
    }

    private async void OnStudyTodayClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<StudyPage>();
        page.DefinirModo(StudyMode.Today);
        // Ao voltar do estudo, só atualiza contadores — não toca a coleção
        page.SessaoConcluida += _vm.AtualizarContadores;
        await Navigation.PushAsync(page);
        page.SessaoConcluida -= _vm.AtualizarContadores;
    }

    private async void OnStudyAllClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<StudyPage>();
        page.DefinirModo(StudyMode.All);
        page.SessaoConcluida += _vm.AtualizarContadores;
        await Navigation.PushAsync(page);
        page.SessaoConcluida -= _vm.AtualizarContadores;
    }

    private async void OnAddCardClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<CreateCardPage>();
        // Ao salvar um novo card, adiciona cirurgicamente na lista
        page.CardSalvo += _vm.AdicionarCard;
        await Navigation.PushAsync(page);
        page.CardSalvo -= _vm.AdicionarCard;
    }

    private async void OnStatsClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(_services.GetRequiredService<StatsPage>());

    private async void OnEditSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipe || swipe.BindingContext is not Card card)
            return;

        var page = _services.GetRequiredService<EditCardPage>();
        page.CarregarCard(card);
        // Ao salvar edição, atualiza o card específico na lista
        page.CardAtualizado += _vm.AtualizarCard;
        await Navigation.PushAsync(page);
        page.CardAtualizado -= _vm.AtualizarCard;
    }

    // OnAppearing NÃO chama LoadCards — elimina o COMException por completo
    // A lista é mantida atualizada via eventos cirúrgicos acima
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.DeletarSolicitado -= OnDeletarSolicitado;
    }
}
