using System.Collections.ObjectModel;
using System.ComponentModel;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly CardService _cardService;

    public event PropertyChangedEventHandler PropertyChanged;

    private ObservableCollection<Card> _cards;
    public ObservableCollection<Card> Cards
    {
        get => _cards;
        set
        {
            _cards = value;
            OnPropertyChanged(nameof(Cards)); // 🔥 avisa a UI
        }
    }

    public HomeViewModel(CardService cardService)
    {
        _cardService = cardService;
        LoadCards();
    }

    public void LoadCards()
    {
        var cards = _cardService.GetAll();
        Cards = new ObservableCollection<Card>(cards);
    }

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}