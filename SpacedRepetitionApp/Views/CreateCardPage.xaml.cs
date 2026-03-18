namespace SpacedRepetitionApp.Views;

public partial class CreateCardPage : ContentPage
{
    private readonly CardService _cardService;

    public CreateCardPage(CardService cardService)
    {
        InitializeComponent(); // ?? precisa existir
        _cardService = cardService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var card = new Card
        {
            Pergunta = PerguntaEntry.Text,
            Resposta = RespostaEntry.Text,
            ProximaRevisao = DateTime.Now
        };

        _cardService.Add(card);

        await DisplayAlert("Sucesso", "Card criado!", "OK");
        await Navigation.PopAsync();
    }
}