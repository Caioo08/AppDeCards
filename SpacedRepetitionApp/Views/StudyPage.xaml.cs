namespace SpacedRepetitionApp.Views;

public partial class StudyPage : ContentPage
{
    private readonly StudyViewModel _vm;
    private bool _modoDefinido = false;

    // Evento disparado quando a sessão conclui — MainPage assina para atualizar contadores
    public event Action? SessaoConcluida;

    public StudyPage(StudyViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        // Monitora quando o ViewModel sinaliza fim de sessão
        _vm.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(StudyViewModel.SessaoConcluida)
                && _vm.SessaoConcluida)
            {
                SessaoConcluida?.Invoke();
            }
        };
    }

    public void DefinirModo(StudyMode modo)
    {
        _modoDefinido = true;
        _vm.IniciarSessao(modo);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_modoDefinido)
            _vm.IniciarSessao(StudyMode.Today);
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();
}
