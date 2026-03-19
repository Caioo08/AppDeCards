using SQLite;

public class Card
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Pergunta { get; set; } = string.Empty;
    public string Resposta { get; set; } = string.Empty;

    public int Intervalo { get; set; } = 1;          // dias até próxima revisão
    public double Facilidade { get; set; } = 2.5;    // fator EF do SM-2
    public int Repeticoes { get; set; } = 0;         // acertos consecutivos

    public DateTime ProximaRevisao { get; set; } = DateTime.Now;
    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public int DeckId { get; set; }

    public int TotalRevisoes { get; set; } = 0;
    public int TotalAcertos { get; set; } = 0;
}
