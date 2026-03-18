using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpacedRepetitionApp.Services
{
    public class ReviewService
    {
        public Card Revisar(Card card, int qualidade)
        {
            if (qualidade < 3)
            {
                card.Repeticoes = 0;
                card.Intervalo = 1;
            }
            else
            {
                card.Repeticoes++;

                if (card.Repeticoes == 1)
                    card.Intervalo = 1;
                else if (card.Repeticoes == 2)
                    card.Intervalo = 6;
                else
                    card.Intervalo = (int)(card.Intervalo * card.Facilidade);
            }

            // Ajusta facilidade
            card.Facilidade += 0.1 - (5 - qualidade) * (0.08 + (5 - qualidade) * 0.02);

            if (card.Facilidade < 1.3)
                card.Facilidade = 1.3;

            // Próxima revisão
            card.ProximaRevisao = DateTime.Now.AddDays(card.Intervalo);

            return card;
        }
    }
}
