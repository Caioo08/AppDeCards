namespace SpacedRepetitionApp.Views;

public partial class CreateCardPage : ContentPage
{
    private readonly CardService _cardService;
    private bool _salvando = false;

    // Evento disparado após salvar — MainPage assina para adicionar o card na lista
    public event Action<Card>? CardSalvo;

    public CreateCardPage(CardService cardService)
    {
        InitializeComponent();
        _cardService = cardService;
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_salvando) return;

        var pergunta = PerguntaEntry.Text?.Trim();
        var resposta = RespostaEntry.Text?.Trim();

        if (string.IsNullOrEmpty(pergunta))
        {
            await DisplayAlert("Atenção", "Preencha a pergunta.", "OK");
            PerguntaEntry.Focus();
            return;
        }

        if (string.IsNullOrEmpty(resposta))
        {
            await DisplayAlert("Atenção", "Preencha a resposta.", "OK");
            RespostaEntry.Focus();
            return;
        }

        _salvando = true;
        try
        {
            var card = new Card
            {
                Pergunta       = pergunta,
                Resposta       = resposta,
                ProximaRevisao = DateTime.Now.Date,
                CriadoEm      = DateTime.Now,
            };

            _cardService.Add(card);

            PerguntaEntry.Text = string.Empty;
            RespostaEntry.Text = string.Empty;

            // Avisa a MainPage ANTES do PopAsync — ainda estamos nesta página
            CardSalvo?.Invoke(card);

            await DisplayAlert("✓", "Card criado!", "OK");
            await Navigation.PopAsync();
        }
        finally
        {
            _salvando = false;
        }
    }
}
