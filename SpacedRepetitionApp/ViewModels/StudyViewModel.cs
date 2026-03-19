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
            OnPropertyChanged(nameof(NaoMostrarResposta));
        }
    }

    public bool NaoMostrarResposta => !_mostrarResposta;

    private bool _sessaoConcluida;
    public bool SessaoConcluida
    {
        get => _sessaoConcluida;
        set
        {
            _sessaoConcluida = value;
            OnPropertyChanged(nameof(SessaoConcluida));
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

        // FIX: Command<string> porque o XAML sempre passa CommandParameter como string.
        // Command<int> faz o comando ignorar o clique silenciosamente quando recebe string.
        ResponderCommand = new Command<string>(qualidadeStr =>
        {
            if (int.TryParse(qualidadeStr, out int qualidade))
                Responder(qualidade);
        });
    }

    private void Responder(int qualidade)
    {
        var card = _cards[_currentIndex];
        var atualizado = _reviewService.Revisar(card, qualidade);
        _cardService.Update(atualizado);

        MostrarResposta = false;
        _currentIndex++;

        if (_currentIndex >= _cards.Count)
        {
            SessaoConcluida = true;
            return;
        }

        AtualizarTela();
    }

    private void AtualizarTela()
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(RespostaAtual));
        OnPropertyChanged(nameof(MostrarResposta));
        OnPropertyChanged(nameof(NaoMostrarResposta));
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
