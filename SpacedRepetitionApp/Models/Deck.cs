using SQLite;

public class Deck
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; }
}