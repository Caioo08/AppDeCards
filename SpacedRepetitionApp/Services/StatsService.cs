using SQLite;

namespace SpacedRepetitionApp.Services;

public class StatsService
{
    private readonly SQLiteConnection _db;
    private const string StreakKey    = "streak";
    private const string LastStudyKey = "last_study";

    public StatsService(DatabaseService database)
    {
        _db = database.GetConnection();
    }

    public int GetStreak()
    {
        var lastStr = Preferences.Get(LastStudyKey, string.Empty);
        if (string.IsNullOrEmpty(lastStr)) return 0;

        var diff = (DateTime.Today - DateTime.Parse(lastStr).Date).TotalDays;
        if (diff > 1) { Preferences.Set(StreakKey, 0); return 0; }
        return Preferences.Get(StreakKey, 0);
    }

    public void RegistrarSessao(int cards, int acertos)
    {
        var hoje = DateTime.Today;
        var sessao = _db.Table<StudySession>().FirstOrDefault(s => s.Data == hoje);
        if (sessao == null)
            _db.Insert(new StudySession { Data = hoje, CardsRevisados = cards, Acertos = acertos });
        else { sessao.CardsRevisados += cards; sessao.Acertos += acertos; _db.Update(sessao); }

        AtualizarStreak();
    }

    private void AtualizarStreak()
    {
        var lastStr = Preferences.Get(LastStudyKey, string.Empty);
        var hoje    = DateTime.Today;

        if (!string.IsNullOrEmpty(lastStr))
        {
            var diff = (hoje - DateTime.Parse(lastStr).Date).TotalDays;
            if      (diff == 1) Preferences.Set(StreakKey, Preferences.Get(StreakKey, 0) + 1);
            else if (diff >  1) Preferences.Set(StreakKey, 1);
        }
        else Preferences.Set(StreakKey, 1);

        Preferences.Set(LastStudyKey, hoje.ToString("O"));
    }

    public (int total, int hoje, double taxaAcerto) GetResumo()
    {
        var total  = _db.Table<Card>().Count();
        var agora  = DateTime.Now;
        var hoje   = _db.Table<Card>().Where(c => c.ProximaRevisao <= agora).Count();
        var cards  = _db.Table<Card>().ToList();
        var rev    = cards.Sum(c => c.TotalRevisoes);
        var ac     = cards.Sum(c => c.TotalAcertos);
        var taxa   = rev > 0 ? Math.Round((double)ac / rev * 100, 1) : 0;
        return (total, hoje, taxa);
    }

    public List<StudySession> GetUltimasSessoes(int n = 7)
        => _db.Table<StudySession>().OrderByDescending(s => s.Data).Take(n).ToList();
}
