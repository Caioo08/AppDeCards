namespace SpacedRepetitionApp.Views;

public partial class CreateCardPage : ContentPage
{
    private readonly CardService _cardService;

    public CreateCardPage(CardService cardService)
    {
        InitializeComponent();
        _cardService = cardService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // FIX: sem validação o app aceitava cards vazios silenciosamente
        if (string.IsNullOrWhiteSpace(PerguntaEntry.Text))
        {
            await DisplayAlert("Atenção", "Preencha a pergunta.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(RespostaEntry.Text))
        {
            await DisplayAlert("Atenção", "Preencha a resposta.", "OK");
            return;
        }

        var card = new Card
        {
            Pergunta = PerguntaEntry.Text.Trim(),
            Resposta = RespostaEntry.Text.Trim(),
            ProximaRevisao = DateTime.Now
        };

        _cardService.Add(card);

        // FIX: campos não eram limpos após salvar
        PerguntaEntry.Text = string.Empty;
        RespostaEntry.Text = string.Empty;

        await DisplayAlert("Sucesso", "Card criado!", "OK");
        await Navigation.PopAsync();
    }
}
