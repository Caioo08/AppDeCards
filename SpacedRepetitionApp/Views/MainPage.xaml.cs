using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        private readonly HomeViewModel _vm;
        private readonly IServiceProvider _services;

        // FIX: em vez de resolver serviços via App.Current.Handler.MauiContext.Services
        // (acoplamento direto ao container, má prática), o IServiceProvider é injetado
        // pelo próprio container — padrão correto para MAUI.
        public MainPage(HomeViewModel vm, IServiceProvider services)
        {
            InitializeComponent();
            BindingContext = vm;
            _vm = vm;
            _services = services;
        }

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                _services.GetRequiredService<CreateCardPage>()
            );
        }

        private async void OnStudyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                _services.GetRequiredService<StudyPage>()
            );
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is HomeViewModel vm)
            {
                vm.LoadCards();
            }
        }
    }
}
