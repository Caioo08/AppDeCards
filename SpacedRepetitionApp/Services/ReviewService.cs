namespace SpacedRepetitionApp.Services;

/// <summary>
/// Implementação completa do SM-2.
/// Cada botão de resposta tem um label de intervalo calculado
/// previamente para mostrar ao usuário ("em 1 dia", "em 4 dias", etc).
/// </summary>
public class ReviewService
{
    // ── Revisão principal ──────────────────────────────────────────────

    public Card Revisar(Card card, int qualidade)
    {
        card.TotalRevisoes++;

        if (qualidade < 3)
        {
            // Falha: reinicia sequência
            card.Repeticoes = 0;
            card.Intervalo = 1;
        }
        else
        {
            card.TotalAcertos++;
            card.Intervalo = card.Repeticoes switch
            {
                0 => 1,
                1 => 6,
                _ => (int)Math.Round(card.Intervalo * card.Facilidade)
            };
            card.Repeticoes++;
            card.Facilidade += 0.1 - (5 - qualidade) * (0.08 + (5 - qualidade) * 0.02);
        }

        card.Facilidade = Math.Max(1.3, card.Facilidade);
        card.ProximaRevisao = DateTime.Now.AddDays(card.Intervalo);
        return card;
    }

    // ── Preview dos próximos intervalos (para mostrar nos botões) ──────

    /// <summary>
    /// Retorna o texto de intervalo que cada botão causaria,
    /// sem modificar o card original.
    /// Ex: "1 dia", "6 dias", "12 dias"
    /// </summary>
    public IntervalPreview CalcularPreview(Card card)
    {
        return new IntervalPreview
        {
            Errei   = FormatarIntervalo(SimularIntervalo(card, 0)),
            Dificil = FormatarIntervalo(SimularIntervalo(card, 3)),
            Medio   = FormatarIntervalo(SimularIntervalo(card, 4)),
            Facil   = FormatarIntervalo(SimularIntervalo(card, 5)),
        };
    }

    private int SimularIntervalo(Card card, int qualidade)
    {
        if (qualidade < 3) return 1;

        return card.Repeticoes switch
        {
            0 => 1,
            1 => 6,
            _ => (int)Math.Round(card.Intervalo * Math.Max(1.3, card.Facilidade + 0.1 - (5 - qualidade) * (0.08 + (5 - qualidade) * 0.02)))
        };
    }

    private string FormatarIntervalo(int dias) => dias switch
    {
        1 => "1 dia",
        _ => $"{dias} dias"
    };
}

public class IntervalPreview
{
    public string Errei   { get; set; } = "";
    public string Dificil { get; set; } = "";
    public string Medio   { get; set; } = "";
    public string Facil   { get; set; } = "";
}
