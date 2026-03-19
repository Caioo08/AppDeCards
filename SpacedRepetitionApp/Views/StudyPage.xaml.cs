namespace SpacedRepetitionApp.Views;

public partial class StudyPage : ContentPage
{
    private readonly StudyViewModel _vm;

    public StudyPage(StudyViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    /// <summary>Chamado pela MainPage antes de PushAsync para definir o modo.</summary>
    public void DefinirModo(StudyMode modo) => _vm.IniciarSessao(modo);

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Só inicia se ainda não foi iniciado (DefinirModo chama IniciarSessao)
        if (!_vm.EmAndamento && !_vm.SessaoConcluida)
            _vm.IniciarSessao(StudyMode.Today);
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();
}
