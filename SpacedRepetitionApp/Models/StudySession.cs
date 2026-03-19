using SQLite;

public class StudySession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Data { get; set; } = DateTime.Today;
    public int CardsRevisados { get; set; } = 0;
    public int Acertos { get; set; } = 0;
}
