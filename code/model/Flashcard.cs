namespace DeckManager.Models;

public class FlashcardModel : BindableObject
{
    private int id;
    private string question = string.Empty;
    private string answer = string.Empty;
    private int successCount;
    private int failureCount;
    private int deckId;

    public int Id
    {
        get => id;
        set { id = value; OnPropertyChanged(); }
    }

    public string Question
    {
        get => question;
        set { question = value; OnPropertyChanged(); }
    }

    public string Answer
    {
        get => answer;
        set { answer = value; OnPropertyChanged(); }
    }

    // CreatedAt ne change jamais après création → pas besoin de notifier
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int SuccessCount
    {
        get => successCount;
        set { successCount = value; OnPropertyChanged(); }
    }

    public int FailureCount
    {
        get => failureCount;
        set { failureCount = value; OnPropertyChanged(); }
    }

    public int DeckId
    {
        get => deckId;
        set { deckId = value; OnPropertyChanged(); }
    }
}