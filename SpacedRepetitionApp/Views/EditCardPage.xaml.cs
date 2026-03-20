namespace SpacedRepetitionApp.Views;

public partial class EditCardPage : ContentPage
{
    private readonly EditCardViewModel _vm;
    private bool _salvando = false;

    // Evento disparado após salvar — MainPage assina para atualizar o card na lista
    public event Action<Card>? CardAtualizado;

    public EditCardPage(EditCardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public void CarregarCard(Card card)
    {
        if (card == null) return;
        _vm.CarregarCard(card);
        PerguntaEntry.Text = card.Pergunta;
        RespostaEntry.Text = card.Resposta;
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_salvando) return;

        var pergunta = PerguntaEntry.Text?.Trim();
        var resposta = RespostaEntry.Text?.Trim();

        if (string.IsNullOrEmpty(pergunta) || string.IsNullOrEmpty(resposta))
        {
            await DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
            return;
        }

        _salvando = true;
        try
        {
            _vm.Pergunta = pergunta;
            _vm.Resposta = resposta;
            _vm.SalvarCommand.Execute(null);

            // Avisa a MainPage ANTES do PopAsync
            // _vm.CardAtual expõe o card atualizado após o Salvar
            CardAtualizado?.Invoke(_vm.CardAtual);

            await DisplayAlert("✓", "Card atualizado!", "OK");
            await Navigation.PopAsync();
        }
        finally
        {
            _salvando = false;
        }
    }
}
