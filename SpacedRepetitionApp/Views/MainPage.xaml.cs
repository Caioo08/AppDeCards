using SpacedRepetitionApp.Views;

namespace SpacedRepetitionApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(HomeViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateCardPage(
                App.Current.Handler.MauiContext.Services.GetService<CardService>()
            ));
        }

        private async void OnStudyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                App.Current.Handler.MauiContext.Services.GetService<StudyPage>()
            );
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is HomeViewModel vm)
            {
                vm.LoadCards(); // 🔥 recarrega ao voltar
            }
        }

    }

}
