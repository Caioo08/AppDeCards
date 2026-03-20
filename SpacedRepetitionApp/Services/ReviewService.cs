namespace SpacedRepetitionApp.Services;

public class ReviewService
{
    // ── Ponto 4: único método privado compartilhado por Revisar e CalcularPreview ──

    /// <summary>
    /// Calcula o próximo intervalo baseado no estado atual do card e na qualidade.
    /// Usado tanto para aplicar a revisão quanto para simular o preview dos botões.
    /// </summary>
    private static (int intervalo, double facilidade) CalcularProximoEstado(
        int repeticoesAtuais, int intervaloAtual, double facilidadeAtual, int qualidade)
    {
        double novaFacilidade = facilidadeAtual;
        int novoIntervalo;

        if (qualidade < 3)
        {
            novoIntervalo = 1;
            // Facilidade não muda em falha (SM-2 puro)
        }
        else
        {
            novoIntervalo = repeticoesAtuais switch
            {
                0 => 1,
                1 => 6,
                _ => (int)Math.Round(intervaloAtual * facilidadeAtual)
            };
            novaFacilidade += 0.1 - (5 - qualidade) * (0.08 + (5 - qualidade) * 0.02);
        }

        novaFacilidade = Math.Max(1.3, novaFacilidade);
        return (novoIntervalo, novaFacilidade);
    }

    // ── Ponto 1: DateTime.Now.Date para consistência sem fuso horário ─────────

    public Card Revisar(Card card, int qualidade)
    {
        card.TotalRevisoes++;

        var (intervalo, facilidade) = CalcularProximoEstado(
            card.Repeticoes, card.Intervalo, card.Facilidade, qualidade);

        if (qualidade < 3)
        {
            card.Repeticoes = 0;
        }
        else
        {
            card.TotalAcertos++;
            card.Repeticoes++;
        }

        card.Intervalo        = intervalo;
        card.Facilidade       = facilidade;
        card.ProximaRevisao   = DateTime.Now.Date.AddDays(intervalo); // Ponto 1: Date normaliza hora para meia-noite
        return card;
    }

    // ── Preview usa exatamente a mesma função interna ─────────────────────────

    public IntervalPreview CalcularPreview(Card card)
    {
        return new IntervalPreview
        {
            Errei   = Formatar(CalcularProximoEstado(card.Repeticoes, card.Intervalo, card.Facilidade, 0).intervalo),
            Dificil = Formatar(CalcularProximoEstado(card.Repeticoes, card.Intervalo, card.Facilidade, 3).intervalo),
            Medio   = Formatar(CalcularProximoEstado(card.Repeticoes, card.Intervalo, card.Facilidade, 4).intervalo),
            Facil   = Formatar(CalcularProximoEstado(card.Repeticoes, card.Intervalo, card.Facilidade, 5).intervalo),
        };
    }

    public static string Formatar(int dias) => dias switch
    {
        1     => "1 dia",
        < 7   => $"{dias} dias",
        7     => "1 semana",
        < 30  => $"{dias / 7} sem.",
        30    => "1 mês",
        < 365 => $"{dias / 30} meses",
        _     => $"{dias / 365} ano(s)"
    };
}

public class IntervalPreview
{
    public string Errei   { get; set; } = "";
    public string Dificil { get; set; } = "";
    public string Medio   { get; set; } = "";
    public string Facil   { get; set; } = "";
}
