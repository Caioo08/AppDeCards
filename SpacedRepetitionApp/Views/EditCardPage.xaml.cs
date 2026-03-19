namespace SpacedRepetitionApp.Views;

public partial class EditCardPage : ContentPage
{
    private readonly EditCardViewModel _vm;

    public EditCardPage(EditCardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public void CarregarCard(Card card)
    {
        _vm.CarregarCard(card);
        PerguntaEntry.Text = card.Pergunta;
        RespostaEntry.Text = card.Resposta;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PerguntaEntry.Text) ||
            string.IsNullOrWhiteSpace(RespostaEntry.Text))
        {
            await DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
            return;
        }

        _vm.Pergunta = PerguntaEntry.Text.Trim();
        _vm.Resposta = RespostaEntry.Text.Trim();
        _vm.SalvarCommand.Execute(null);

        await DisplayAlert("✓", "Card atualizado!", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();
}
