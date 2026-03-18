using SQLite;
using System.IO;

public class DatabaseService
{
    private SQLiteConnection _db;

    public DatabaseService()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "cards.db");
        _db = new SQLiteConnection(path);

        _db.CreateTable<Card>();
        _db.CreateTable<Deck>();
    }

    public SQLiteConnection GetConnection()
    {
        return _db;
    }
}