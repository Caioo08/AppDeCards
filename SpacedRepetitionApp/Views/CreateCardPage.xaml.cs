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

        _cardService.Add(new Card
        {
            Pergunta       = PerguntaEntry.Text.Trim(),
            Resposta       = RespostaEntry.Text.Trim(),
            ProximaRevisao = DateTime.Now,
            CriadoEm      = DateTime.Now
        });

        PerguntaEntry.Text = string.Empty;
        RespostaEntry.Text = string.Empty;

        await DisplayAlert("✓", "Card criado!", "OK");
        await Navigation.PopAsync();
    }
}
