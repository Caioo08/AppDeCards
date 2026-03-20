namespace SpacedRepetitionApp.Views;

public partial class StatsPage : ContentPage
{
    private readonly StatsViewModel _vm;

    public StatsPage(StatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Carregar();
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();
}
