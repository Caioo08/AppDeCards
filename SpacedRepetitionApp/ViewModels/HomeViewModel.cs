using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SpacedRepetitionApp.Services;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly CardService _cardService;
    private readonly StatsService _statsService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<Card> _cards = new();
    public ObservableCollection<Card> Cards
    {
        get => _cards;
        set { _cards = value; OnPropertyChanged(nameof(Cards)); }
    }

    private int _cardsHoje;
    public int CardsHoje
    {
        get => _cardsHoje;
        set { _cardsHoje = value; OnPropertyChanged(nameof(CardsHoje)); OnPropertyChanged(nameof(TemCardsHoje)); }
    }

    private int _streak;
    public int Streak
    {
        get => _streak;
        set { _streak = value; OnPropertyChanged(nameof(Streak)); }
    }

    public bool TemCardsHoje => CardsHoje > 0;

    // Controla se a resposta está visível nos cards da lista
    // A resposta fica SEMPRE oculta na home — isso é intencional
    // (o usuário só vê a resposta na tela de estudo)
    public bool RespostaVisivel => false;

    public ICommand DeleteCommand { get; }
    public ICommand RecarregarCommand { get; }

    public HomeViewModel(CardService cardService, StatsService statsService)
    {
        _cardService = cardService;
        _statsService = statsService;
        DeleteCommand    = new Command<Card>(DeletarCard);
        RecarregarCommand = new Command(LoadCards);
        LoadCards();
    }

    public void LoadCards()
    {
        Cards      = new ObservableCollection<Card>(_cardService.GetAll());
        CardsHoje  = _cardService.GetTodayCount();
        Streak     = _statsService.GetStreak();
    }

    private void DeletarCard(Card card)
    {
        if (card == null) return;
        _cardService.Delete(card);
        Cards.Remove(card);
        CardsHoje = _cardService.GetTodayCount();
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
