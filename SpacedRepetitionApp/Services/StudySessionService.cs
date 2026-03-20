namespace SpacedRepetitionApp.Services;

/// <summary>
/// Ponto 3: lógica de sessão extraída do StudyViewModel.
/// Responsável por: estado da sessão, métricas (acertos, tempo, total)
/// e integração com StatsService.
/// O ViewModel apenas delega e expõe propriedades para a UI.
/// </summary>
public class StudySessionService
{
    private readonly CardService   _cardService;
    private readonly ReviewService _reviewService;
    private readonly StatsService  _statsService;

    private List<Card> _cards        = new();
    private int        _currentIndex = 0;
    private int        _acertos      = 0;
    private DateTime   _inicio       = DateTime.Now;

    // ── Estado da sessão ─────────────────────────────────────────────

    public bool     Iniciada      => _cards.Count > 0;
    public bool     SemCards      => _cards.Count == 0;
    public bool     Concluida     { get; private set; }
    public int      TotalCards    => _cards.Count;
    public int      Acertos       => _acertos;
    public int      IndexAtual    => _currentIndex;

    public Card?    CardAtual     => EmAndamento ? _cards[_currentIndex] : null;
    public bool     EmAndamento   => !Concluida && _cards.Count > 0 && _currentIndex < _cards.Count;

    public double   ProgressoValor => _cards.Count > 0
        ? (double)_currentIndex / _cards.Count
        : 0;

    public string   Progresso      => _cards.Count > 0
        ? $"{_currentIndex + 1} de {_cards.Count}"
        : string.Empty;

    public string   TaxaAcertoTexto => TotalCards > 0
        ? $"{Math.Round((double)_acertos / TotalCards * 100)}%"
        : "0%";

    // ── Construtor ───────────────────────────────────────────────────

    public StudySessionService(CardService cardService, ReviewService reviewService, StatsService statsService)
    {
        _cardService   = cardService;
        _reviewService = reviewService;
        _statsService  = statsService;
    }

    // ── Controle de sessão ───────────────────────────────────────────

    public void Iniciar(StudyMode modo)
    {
        _currentIndex = 0;
        _acertos      = 0;
        _inicio       = DateTime.Now;
        Concluida     = false;

        // Ponto 9: NÃO cai em All automaticamente — respeita o modo escolhido
        _cards = modo == StudyMode.All
            ? _cardService.GetAll()
            : _cardService.GetTodayCards();

        // Se Today e não há cards vencidos, SemCards = true, sessão encerra corretamente
    }

    /// <summary>
    /// Aplica a revisão no card atual e avança o índice.
    /// Retorna true se a sessão foi concluída nesta resposta.
    /// </summary>
    public bool Responder(int qualidade)
    {
        if (!EmAndamento) return false;

        // Ponto 10: null check defensivo
        var card = _cards[_currentIndex];
        if (card == null) return false;

        _cardService.Update(_reviewService.Revisar(card, qualidade));
        if (qualidade >= 3) _acertos++;

        _currentIndex++;

        if (_currentIndex >= _cards.Count)
        {
            _statsService.RegistrarSessao(TotalCards, _acertos);
            Concluida = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Preview dos intervalos do card atual para exibir nos botões.
    /// </summary>
    public IntervalPreview? GetPreviewAtual()
    {
        // Ponto 10: guarda safely antes de acessar
        var card = CardAtual;
        return card is null ? null : _reviewService.CalcularPreview(card);
    }
}
