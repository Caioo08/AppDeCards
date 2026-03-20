using SQLite;

public class CardService
{
    private readonly SQLiteConnection _db;

    public CardService(DatabaseService database)
    {
        _db = database.GetConnection();
    }

    public void   Add(Card card)    => _db.Insert(card);
    public void   Update(Card card) => _db.Update(card);
    public void   Delete(Card card) => _db.Delete(card);
    public Card?  GetById(int id)   => _db.Find<Card>(id);

    // ── Ponto 8: ordenação por ProximaRevisao → Facilidade → Intervalo ───────
    // Objetivo: cards mais atrasados e mais difíceis aparecem primeiro

    public List<Card> GetAll()
        => _db.Table<Card>()
              .ToList()
              .OrderBy(c => c.ProximaRevisao)
              .ThenBy(c => c.Facilidade)      // mais difíceis (EF menor) primeiro
              .ThenBy(c => c.Intervalo)
              .ToList();

    // ── Ponto 1: DateTime.Now.Date normaliza a comparação sem hora ─────────────

    public List<Card> GetTodayCards()
    {
        var hoje = DateTime.Now.Date;
        return _db.Table<Card>()
                  .Where(c => c.ProximaRevisao <= hoje)
                  .ToList()
                  .OrderBy(c => c.ProximaRevisao)
                  .ThenBy(c => c.Facilidade)
                  .ThenBy(c => c.Intervalo)
                  .ToList();
    }

    public int GetTodayCount()
    {
        var hoje = DateTime.Now.Date;
        return _db.Table<Card>().Where(c => c.ProximaRevisao <= hoje).Count();
    }
}
