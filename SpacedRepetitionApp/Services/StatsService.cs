using SQLite;

namespace SpacedRepetitionApp.Services;

public class StatsService
{
    private readonly SQLiteConnection _db;

    public StatsService(DatabaseService database)
    {
        _db = database.GetConnection();
    }

    // ── Ponto 2: streak calculado 100% a partir do SQLite, sem Preferences ────

    /// <summary>
    /// Calcula o streak contando dias consecutivos com sessão registrada,
    /// de hoje para trás. Única fonte de verdade: tabela StudySession.
    /// </summary>
    public int GetStreak()
    {
        // Ponto 1: usa .Date para comparar apenas a data, sem hora
        var hoje    = DateTime.Now.Date;
        var sessoes = _db.Table<StudySession>()
                         .OrderByDescending(s => s.Data)
                         .ToList()
                         .Select(s => s.Data.Date)
                         .Distinct()
                         .ToList();

        if (sessoes.Count == 0) return 0;

        // Streak só conta se a última sessão foi hoje ou ontem
        var maisRecente = sessoes[0];
        if ((hoje - maisRecente).TotalDays > 1) return 0;

        int streak = 0;
        var diaEsperado = hoje;

        // Se a sessão mais recente foi ontem, começamos de ontem
        if (maisRecente < hoje) diaEsperado = maisRecente;

        foreach (var data in sessoes)
        {
            if (data.Date == diaEsperado)
            {
                streak++;
                diaEsperado = diaEsperado.AddDays(-1);
            }
            else break; // sequência quebrada
        }

        return streak;
    }

    // ── Ponto 1: DateTime.Now.Date em todas as comparações ────────────────────

    public void RegistrarSessao(int cards, int acertos)
    {
        var hoje   = DateTime.Now.Date;
        var sessao = _db.Table<StudySession>()
                        .FirstOrDefault(s => s.Data == hoje);

        if (sessao == null)
            _db.Insert(new StudySession { Data = hoje, CardsRevisados = cards, Acertos = acertos });
        else
        {
            sessao.CardsRevisados += cards;
            sessao.Acertos        += acertos;
            _db.Update(sessao);
        }
    }

    public (int total, int hoje, double taxaAcerto) GetResumo()
    {
        var agora = DateTime.Now.Date; // Ponto 1: comparação por data
        var total = _db.Table<Card>().Count();
        var hoje  = _db.Table<Card>().Where(c => c.ProximaRevisao <= agora).Count();
        var cards = _db.Table<Card>().ToList();
        var rev   = cards.Sum(c => c.TotalRevisoes);
        var ac    = cards.Sum(c => c.TotalAcertos);
        var taxa  = rev > 0 ? Math.Round((double)ac / rev * 100, 1) : 0.0;
        return (total, hoje, taxa);
    }

    public List<StudySession> GetUltimasSessoes(int n = 7)
        => _db.Table<StudySession>()
              .OrderByDescending(s => s.Data)
              .Take(n)
              .ToList();
}
