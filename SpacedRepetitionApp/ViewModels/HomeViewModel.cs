using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SpacedRepetitionApp.Services;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly CardService  _cardService;
    private readonly StatsService _statsService;

    public event PropertyChangedEventHandler? PropertyChanged;

    // Coleção única — nunca substituída, nunca limpa fora da main thread
    public ObservableCollection<Card> Cards { get; } = new();

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

    private bool _deletando = false;

    public ICommand DeleteCommand     { get; }
    public ICommand RecarregarCommand { get; }

    public event Action<Card>? DeletarSolicitado;

    public HomeViewModel(CardService cardService, StatsService statsService)
    {
        _cardService  = cardService;
        _statsService = statsService;
        DeleteCommand     = new Command<Card>(card => DeletarSolicitado?.Invoke(card));
        RecarregarCommand = new Command(CarregarInicial);

        // Carga inicial — segura porque acontece antes de qualquer UI existir
        CarregarInicial();
    }

    // Chamado apenas UMA vez na inicialização do app
    private void CarregarInicial()
    {
        var lista  = _cardService.GetAll();
        var hoje   = _cardService.GetTodayCount();
        var streak = _statsService.GetStreak();

        Cards.Clear();
        foreach (var c in lista)
            Cards.Add(c);

        CardsHoje = hoje;
        Streak    = streak;
    }

    // Adiciona um card recém-criado no topo da lista, sem recarregar tudo
    public void AdicionarCard(Card card)
    {
        Cards.Insert(0, card);
        CardsHoje = _cardService.GetTodayCount();
    }

    // Atualiza um card editado na lista sem recarregar tudo
    public void AtualizarCard(Card card)
    {
        var index = Cards.IndexOf(Cards.FirstOrDefault(c => c.Id == card.Id));
        if (index >= 0)
        {
            Cards.RemoveAt(index);
            Cards.Insert(index, card);
        }
        CardsHoje = _cardService.GetTodayCount();
    }

    // Atualiza apenas os contadores após uma sessão de estudo
    public void AtualizarContadores()
    {
        CardsHoje = _cardService.GetTodayCount();
        Streak    = _statsService.GetStreak();
    }

    public void ConfirmarDelete(Card card)
    {
        if (_deletando || card == null) return;
        _deletando = true;
        try
        {
            _cardService.Delete(card);
            var item = Cards.FirstOrDefault(c => c.Id == card.Id);
            if (item != null) Cards.Remove(item);
            CardsHoje = _cardService.GetTodayCount();
        }
        finally
        {
            _deletando = false;
        }
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
