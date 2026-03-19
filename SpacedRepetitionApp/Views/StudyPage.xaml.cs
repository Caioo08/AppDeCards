namespace SpacedRepetitionApp.Views;

public partial class StudyPage : ContentPage
{
    public StudyPage(StudyViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // FIX: handler para o botão Voltar na tela de sessão concluída
    private async void OnVoltarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();
}
