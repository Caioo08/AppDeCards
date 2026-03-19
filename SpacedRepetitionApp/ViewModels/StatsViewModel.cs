using System.ComponentModel;
using SpacedRepetitionApp.Services;

public class StatsViewModel : INotifyPropertyChanged
{
    private readonly StatsService _statsService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int    TotalCards       { get; private set; }
    public int    CardsHoje        { get; private set; }
    public double TaxaAcerto       { get; private set; }
    public int    Streak           { get; private set; }
    public List<StudySession> UltimasSessoes { get; private set; } = new();

    public string TaxaAcertoTexto => $"{TaxaAcerto}%";
    public string StreakTexto     => Streak == 1 ? "1 dia seguido" : $"{Streak} dias seguidos";

    public StatsViewModel(StatsService statsService)
    {
        _statsService = statsService;
        Carregar();
    }

    public void Carregar()
    {
        var (total, hoje, taxa) = _statsService.GetResumo();
        TotalCards      = total;
        CardsHoje       = hoje;
        TaxaAcerto      = taxa;
        Streak          = _statsService.GetStreak();
        UltimasSessoes  = _statsService.GetUltimasSessoes(7);

        OnPropertyChanged(nameof(TotalCards));
        OnPropertyChanged(nameof(CardsHoje));
        OnPropertyChanged(nameof(TaxaAcerto));
        OnPropertyChanged(nameof(TaxaAcertoTexto));
        OnPropertyChanged(nameof(Streak));
        OnPropertyChanged(nameof(StreakTexto));
        OnPropertyChanged(nameof(UltimasSessoes));
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
