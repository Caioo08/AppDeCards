using SQLite;

public class Card
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Pergunta { get; set; }
    public string Resposta { get; set; }

    public int Intervalo { get; set; } = 1;
    public double Facilidade { get; set; } = 2.5;
    public int Repeticoes { get; set; } = 0;

    public DateTime ProximaRevisao { get; set; } = DateTime.Now;

    public int DeckId { get; set; }
}