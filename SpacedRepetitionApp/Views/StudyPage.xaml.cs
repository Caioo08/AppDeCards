namespace SpacedRepetitionApp.Views;

public partial class StudyPage : ContentPage
{
    public StudyPage(StudyViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}