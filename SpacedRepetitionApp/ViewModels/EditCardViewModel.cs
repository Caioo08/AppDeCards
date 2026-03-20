using System.ComponentModel;
using System.Windows.Input;

public class EditCardViewModel : INotifyPropertyChanged
{
    private readonly CardService _cardService;
    private Card _card = null!;

    public event PropertyChangedEventHandler? PropertyChanged;

    // Expõe o card após salvar para o evento CardAtualizado da View
    public Card CardAtual => _card;

    private string _pergunta = string.Empty;
    public string Pergunta
    {
        get => _pergunta;
        set { _pergunta = value; OnPropertyChanged(nameof(Pergunta)); }
    }

    private string _resposta = string.Empty;
    public string Resposta
    {
        get => _resposta;
        set { _resposta = value; OnPropertyChanged(nameof(Resposta)); }
    }

    public ICommand SalvarCommand { get; }

    public EditCardViewModel(CardService cardService)
    {
        _cardService  = cardService;
        SalvarCommand = new Command(Salvar);
    }

    public void CarregarCard(Card card)
    {
        _card    = card;
        Pergunta = card.Pergunta;
        Resposta = card.Resposta;
    }

    private void Salvar()
    {
        _card.Pergunta = Pergunta.Trim();
        _card.Resposta = Resposta.Trim();
        _cardService.Update(_card);
    }

    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
