using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SpacedRepetitionApp.Services;

public class StudyViewModel : INotifyPropertyChanged
{
    private readonly CardService _cardService;
    private readonly ReviewService _reviewService;

    public event PropertyChangedEventHandler PropertyChanged;

    private List<Card> _cards;
    private int _currentIndex = 0;

    public string PerguntaAtual => _cards.Count > 0 ? _cards[_currentIndex].Pergunta : "Sem cards";
    public string RespostaAtual => _cards.Count > 0 ? _cards[_currentIndex].Resposta : "";

    private bool _mostrarResposta;
    public bool MostrarResposta
    {
        get => _mostrarResposta;
        set
        {
            _mostrarResposta = value;
            OnPropertyChanged(nameof(MostrarResposta));
        }
    }

    public ICommand MostrarRespostaCommand { get; }
    public ICommand ResponderCommand { get; }

    public StudyViewModel(CardService cardService, ReviewService reviewService)
    {
        _cardService = cardService;
        _reviewService = reviewService;

        _cards = _cardService.GetTodayCards();
        if (_cards.Count == 0)
            _cards = _cardService.GetAll();

        MostrarRespostaCommand = new Command(() => MostrarResposta = true);
        ResponderCommand = new Command<int>(Responder);
    }

    private void Responder(int qualidade)
    {
        var card = _cards[_currentIndex];

        var atualizado = _reviewService.Revisar(card, qualidade);

        _cardService.Update(atualizado);

        MostrarResposta = false;
        _currentIndex++;

        if (_currentIndex >= _cards.Count)
            _currentIndex = 0;

        AtualizarTela();
    }

    private void AtualizarTela()
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(RespostaAtual));
        OnPropertyChanged(nameof(MostrarResposta));
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}