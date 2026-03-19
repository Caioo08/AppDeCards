using SQLite;

public class CardService
{
    private readonly SQLiteConnection _db;

    public CardService(DatabaseService database)
    {
        _db = database.GetConnection();
    }

    public void Add(Card card)
    {
        _db.Insert(card);
    }

    public List<Card> GetAll()
    {
        return _db.Table<Card>().ToList();
    }

    public List<Card> GetTodayCards()
    {
        // FIX: sqlite-net-pcl não suporta DateTime.Now diretamente no LINQ —
        // causa NotSupportedException em runtime. Usar variável local.
        var agora = DateTime.Now;
        return _db.Table<Card>()
                  .Where(c => c.ProximaRevisao <= agora)
                  .ToList();
    }

    public void Update(Card card)
    {
        _db.Update(card);
    }
}
