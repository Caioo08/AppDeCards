using SQLite;
using System.Collections.Generic;
using System.Linq;

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
        return _db.Table<Card>()
                  .Where(c => c.ProximaRevisao <= DateTime.Now)
                  .ToList();
    }

    public void Update(Card card)
    {
        _db.Update(card);
    }
}