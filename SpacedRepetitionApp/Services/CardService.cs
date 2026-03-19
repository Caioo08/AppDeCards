using SQLite;

public class CardService
{
    private readonly SQLiteConnection _db;

    public CardService(DatabaseService database)
    {
        _db = database.GetConnection();
    }

    public void Add(Card card)       => _db.Insert(card);
    public void Update(Card card)    => _db.Update(card);
    public void Delete(Card card)    => _db.Delete(card);
    public Card? GetById(int id)     => _db.Find<Card>(id);

    public List<Card> GetAll()       => _db.Table<Card>().ToList();

    /// <summary>Cards com ProximaRevisao vencida (modo revisão normal).</summary>
    public List<Card> GetTodayCards()
    {
        var agora = DateTime.Now;
        return _db.Table<Card>()
                  .Where(c => c.ProximaRevisao <= agora)
                  .ToList();
    }

    public int GetTodayCount()
    {
        var agora = DateTime.Now;
        return _db.Table<Card>().Where(c => c.ProximaRevisao <= agora).Count();
    }
}
