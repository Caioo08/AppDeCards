using System.ComponentModel;
using System.Windows.Input;
using SpacedRepetitionApp.Services;

public enum StudyMode { Today, All }

public class StudyViewModel : INotifyPropertyChanged
{
    private readonly StudySessionService _sessao;

    public event PropertyChangedEventHandler? PropertyChanged;

    // Ponto 7: guard contra múltiplos cliques
    private bool _isBusy = false;

    // ── Propriedades delegadas ao StudySessionService ─────────────────

    public string PerguntaAtual  => _sessao.CardAtual?.Pergunta  ?? string.Empty;
    public string RespostaAtual  => _sessao.CardAtual?.Resposta  ?? string.Empty;
    public string Progresso      => _sessao.Progresso;
    public double ProgressoValor => _sessao.ProgressoValor;
    public bool   EmAndamento    => _sessao.EmAndamento;
    public bool   SemCards       => _sessao.SemCards;
    public bool   SessaoConcluida => _sessao.Concluida;
    public string TaxaAcertoTexto => _sessao.TaxaAcertoTexto;
    public int    TotalSessao    => _sessao.TotalCards;
    public int    AcertosSessao  => _sessao.Acertos;
    public string ModoTexto      => _modo == StudyMode.All ? "Estudando todos" : "Revisão do dia";

    private StudyMode _modo = StudyMode.Today;

    // ── Preview ───────────────────────────────────────────────────────

    private IntervalPreview _preview = new();

    public string PreviewErrei   => $"✗  Errei\n{_preview.Errei}";
    public string PreviewDificil => $"◐  Difícil\n{_preview.Dificil}";
    public string PreviewMedio   => $"◑  Médio\n{_preview.Medio}";
    public string PreviewFacil   => $"✓  Fácil\n{_preview.Facil}";

    // ── Resposta ──────────────────────────────────────────────────────

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

    // ── Commands ──────────────────────────────────────────────────────

    public ICommand MostrarRespostaCommand { get; }
    public ICommand ResponderCommand       { get; }
    public ICommand ReiniciarCommand       { get; }

    // ── Construtor ────────────────────────────────────────────────────

    public StudyViewModel(StudySessionService sessao)
    {
        _sessao = sessao;

        MostrarRespostaCommand = new Command(
            () => MostrarResposta = true,
            () => !_isBusy && _sessao.EmAndamento);

        // Ponto 7: IsBusy bloqueia múltiplos cliques rápidos
        ResponderCommand = new Command<string>(qualStr =>
        {
            if (_isBusy) return;
            if (!int.TryParse(qualStr, out int q)) return;

            _isBusy = true;
            try { Responder(q); }
            finally { _isBusy = false; }
        });

        ReiniciarCommand = new Command(() => IniciarSessao(_modo));
    }

    // ── Sessão ────────────────────────────────────────────────────────

    public void IniciarSessao(StudyMode modo = StudyMode.Today)
    {
        _modo           = modo;
        MostrarResposta = false;
        _sessao.Iniciar(modo);
        AtualizarPreview();
        AtualizarTela();
    }

    private void Responder(int qualidade)
    {
        MostrarResposta = false;

        // Ponto 10: delegate retorna bool indicando se sessão concluiu
        var concluida = _sessao.Responder(qualidade);

        if (concluida)
        {
            AtualizarTela();
            OnPropertyChanged(nameof(TaxaAcertoTexto));
            OnPropertyChanged(nameof(AcertosSessao));
            OnPropertyChanged(nameof(TotalSessao));
            OnPropertyChanged(nameof(SessaoConcluida));
            return;
        }

        AtualizarPreview();
        AtualizarTela();
    }

    private void AtualizarPreview()
    {
        // Ponto 10: null safety — preview pode ser null se não há card atual
        _preview = _sessao.GetPreviewAtual() ?? new IntervalPreview();
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
        OnPropertyChanged(nameof(SessaoConcluida));
        OnPropertyChanged(nameof(ModoTexto));
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
