using System.ComponentModel;
using System.Windows.Input;
using SpacedRepetitionApp.Services;

/// <summary>
/// Modo de estudo controlado por StudyMode:
///   Today  = apenas cards com revisão vencida (padrão)
///   All    = todos os cards (opção "Estudar Tudo")
/// </summary>
public enum StudyMode { Today, All }

public class StudyViewModel : INotifyPropertyChanged
{
    private readonly CardService   _cardService;
    private readonly ReviewService _reviewService;
    private readonly StatsService  _statsService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private List<Card> _cards = new();
    private int        _currentIndex = 0;
    private int        _acertos      = 0;
    private StudyMode  _modo         = StudyMode.Today;

    // ── Propriedades da pergunta/resposta ─────────────────────────────

    public string PerguntaAtual => _cards.Count > 0 ? _cards[_currentIndex].Pergunta : string.Empty;
    public string RespostaAtual => _cards.Count > 0 ? _cards[_currentIndex].Resposta : string.Empty;

    public string Progresso      => _cards.Count > 0 ? $"{_currentIndex + 1} de {_cards.Count}" : string.Empty;
    public double ProgressoValor => _cards.Count > 0 ? (double)_currentIndex / _cards.Count : 0;

    public string ModoTexto => _modo == StudyMode.All ? "Estudando todos" : "Revisão do dia";

    // ── Preview de intervalos (mostrado nos botões) ───────────────────

    private IntervalPreview _preview = new();

    public string PreviewErrei   => $"✗  Errei\n{_preview.Errei}";
    public string PreviewDificil => $"◐  Difícil\n{_preview.Dificil}";
    public string PreviewMedio   => $"◑  Médio\n{_preview.Medio}";
    public string PreviewFacil   => $"✓  Fácil\n{_preview.Facil}";

    // ── Estado da UI ──────────────────────────────────────────────────

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
            OnPropertyChanged(nameof(EmAndamento));
        }
    }

    public bool EmAndamento => !SessaoConcluida && _cards.Count > 0;
    public bool SemCards    => _cards.Count == 0;

    // Resultado da sessão
    public int    TotalSessao      => _cards.Count;
    public int    AcertosSessao    => _acertos;
    public string TaxaAcertoTexto  => TotalSessao > 0
        ? $"{Math.Round((double)_acertos / TotalSessao * 100)}%"
        : "0%";

    // ── Commands ──────────────────────────────────────────────────────

    public ICommand MostrarRespostaCommand { get; }
    public ICommand ResponderCommand       { get; }
    public ICommand ReiniciarCommand       { get; }

    // ── Construtor ────────────────────────────────────────────────────

    public StudyViewModel(CardService cardService, ReviewService reviewService, StatsService statsService)
    {
        _cardService   = cardService;
        _reviewService = reviewService;
        _statsService  = statsService;

        MostrarRespostaCommand = new Command(() => MostrarResposta = true);

        ResponderCommand = new Command<string>(qualStr =>
        {
            if (int.TryParse(qualStr, out int q)) Responder(q);
        });

        ReiniciarCommand = new Command(() => IniciarSessao(_modo));
    }

    // ── Iniciar sessão ────────────────────────────────────────────────

    public void IniciarSessao(StudyMode modo = StudyMode.Today)
    {
        _modo         = modo;
        _currentIndex = 0;
        _acertos      = 0;
        SessaoConcluida = false;
        MostrarResposta = false;

        _cards = modo == StudyMode.All
            ? _cardService.GetAll()
            : _cardService.GetTodayCards();

        // Se não há cards vencidos, cai em All automaticamente
        if (_cards.Count == 0 && modo == StudyMode.Today)
            _cards = _cardService.GetAll();

        AtualizarPreview();
        AtualizarTela();
    }

    // ── Responder ─────────────────────────────────────────────────────

    private void Responder(int qualidade)
    {
        var card = _cards[_currentIndex];
        _cardService.Update(_reviewService.Revisar(card, qualidade));

        if (qualidade >= 3) _acertos++;

        MostrarResposta = false;
        _currentIndex++;

        if (_currentIndex >= _cards.Count)
        {
            _statsService.RegistrarSessao(_cards.Count, _acertos);
            SessaoConcluida = true;
            OnPropertyChanged(nameof(TaxaAcertoTexto));
            OnPropertyChanged(nameof(AcertosSessao));
            OnPropertyChanged(nameof(TotalSessao));
            return;
        }

        AtualizarPreview();
        AtualizarTela();
    }

    private void AtualizarPreview()
    {
        if (_cards.Count == 0 || _currentIndex >= _cards.Count) return;
        _preview = _reviewService.CalcularPreview(_cards[_currentIndex]);
        OnPropertyChanged(nameof(PreviewErrei));
        OnPropertyChanged(nameof(PreviewDificil));
        OnPropertyChanged(nameof(PreviewMedio));
        OnPropertyChanged(nameof(PreviewFacil));
    }

    private void AtualizarTela()
    {
        OnPropertyChanged(nameof(PerguntaAtual));
        OnPropertyChanged(nameof(RespostaAtual));
        OnPropertyChanged(nameof(Progresso));
        OnPropertyChanged(nameof(ProgressoValor));
        OnPropertyChanged(nameof(EmAndamento));
        OnPropertyChanged(nameof(SemCards));
        OnPropertyChanged(nameof(ModoTexto));
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
