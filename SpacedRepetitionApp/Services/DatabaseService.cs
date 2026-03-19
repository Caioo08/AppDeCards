using SQLite;

public class DatabaseService
{
    private readonly SQLiteConnection _db;

    public DatabaseService()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "cards.db");
        _db = new SQLiteConnection(path);

        _db.CreateTable<Card>();
        _db.CreateTable<Deck>();
        _db.CreateTable<StudySession>();

        _db.CreateIndex("idx_card_proxima", "Card", "ProximaRevisao");
        _db.CreateIndex("idx_session_data", "StudySession", "Data");
    }

    public SQLiteConnection GetConnection() => _db;
}
