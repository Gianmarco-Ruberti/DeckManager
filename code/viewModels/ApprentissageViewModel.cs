using System.Collections.ObjectModel;
using DeckManager.Models;
using DeckManager.Services;

namespace DeckManager.ViewModels;

public class ApprentissageViewModel : BindableObject
{
    private FlashcardModel? currentFlashcard;
    private int currentIndex = 0;
    private double progress = 0;
    private DateTime startTime;

    public ObservableCollection<FlashcardModel> Flashcards { get; set; }

    public FlashcardModel? CurrentFlashcard
    {
        get => currentFlashcard;
        set { currentFlashcard = value; OnPropertyChanged(); }
    }

    public double Progress
    {
        get => progress;
        set { progress = value; OnPropertyChanged(); }
    }

    public ApprentissageViewModel()
    {
        // On initialise la session dès le départ
        ReinitialiserSession();
    }

    // CETTE MÉTHODE EST LA CLÉ : Elle remet tout à zéro
    public void ReinitialiserSession()
    {
        currentIndex = 0;
        Progress = 0;
        startTime = DateTime.Now;

        // On récupère les cartes fraîches du service
        Flashcards = FlashcardService.Flashcards;

        if (Flashcards != null && Flashcards.Count > 0)
        {
            CurrentFlashcard = Flashcards[0];
        }
        else
        {
            CurrentFlashcard = new FlashcardModel { Question = "Aucune carte trouvée", Answer = "Créez des cartes d'abord !" };
        }
    }

    public void MarkAsSuccess()
    {
        if (CurrentFlashcard != null && currentIndex < Flashcards.Count)
        {
            CurrentFlashcard.SuccessCount++;
            FlashcardService.SaveFlashcards();
            NextFlashcard();
        }
    }

    public void MarkAsFailed()
    {
        if (CurrentFlashcard != null && currentIndex < Flashcards.Count)
        {
            CurrentFlashcard.FailureCount++;
            FlashcardService.SaveFlashcards();
            NextFlashcard();
        }
    }

    private void NextFlashcard()
    {
        currentIndex++;
        if (Flashcards == null || Flashcards.Count == 0) return;

        Progress = (double)currentIndex / Flashcards.Count;

        if (currentIndex < Flashcards.Count)
        {
            CurrentFlashcard = Flashcards[currentIndex];
        }
        else
        {
            SessionTerminee();
        }
    }

    private void SessionTerminee()
    {
        var totalTime = DateTime.Now - startTime;
        var difficultCard = Flashcards.OrderByDescending(f => f.FailureCount).FirstOrDefault();
        string difficultCardName = difficultCard?.Question ?? "N/A";
        int perfectCards = Flashcards.Count(f => f.FailureCount == 0);
        double memoPercent = Flashcards.Count > 0 ? (double)perfectCards / Flashcards.Count * 100 : 0;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"{nameof(ResumePage)}?" +
                $"Time={totalTime.Minutes}m {totalTime.Seconds}s&" +
                $"Difficult={difficultCardName}&" +
                $"Perfect={perfectCards}&" +
                $"Percent={memoPercent:F0}");
        });
    }
}