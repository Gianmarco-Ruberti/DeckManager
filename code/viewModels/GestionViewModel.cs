using System.Collections.ObjectModel;
using DeckManager.Models;
using DeckManager.Services;

namespace DeckManager.ViewModels;

public class GestionViewModel : BindableObject
{
    private FlashcardModel currentFlashcard = new();

    public FlashcardModel CurrentFlashcard
    {
        get => currentFlashcard;
        set { currentFlashcard = value; OnPropertyChanged(); }
    }

    // Pointe vers la collection partagée
    public ObservableCollection<FlashcardModel> Flashcards => FlashcardService.Flashcards;

    private Deck selectedDeck;
    public Deck SelectedDeck
    {
        get => selectedDeck;
        set
        {
            selectedDeck = value;
            OnPropertyChanged();
            LoadFlashcardsForSelectedDeck();
        }
    }

    public ObservableCollection<FlashcardModel> DeckFlashcards { get; } = new();

    private void LoadFlashcardsForSelectedDeck()
    {
        if (SelectedDeck != null)
        {
            DeckFlashcards.Clear();
            var flashcards = FlashcardService.GetFlashcardsByDeck(SelectedDeck.Id);
            foreach (var card in flashcards)
            {
                DeckFlashcards.Add(card);
            }
        }
    }

    public bool SaveFlashcard()
    {
        if (string.IsNullOrWhiteSpace(CurrentFlashcard.Question) ||
            string.IsNullOrWhiteSpace(CurrentFlashcard.Answer))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
                await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK"));
            return false;
        }

        // Ajout via le service
        FlashcardService.Add(CurrentFlashcard);

        // --- SAUVEGARDE SUR LE DISQUE ---
        FlashcardService.SaveFlashcards();

        CurrentFlashcard = new FlashcardModel();
        return true;
    }
    public async void OnEditClicked(FlashcardModel selectedPath)
    {
        // On passe la carte au BindingContext de la page suivante
        var editPage = new EditPage();
        editPage.BindingContext = this;
        this.CurrentFlashcard = selectedPath; // On définit la carte actuelle sur celle cliquée

        await Shell.Current.Navigation.PushAsync(editPage);
    }

    public void DeleteFlashcard(FlashcardModel flashcard)
    {
        FlashcardService.Delete(flashcard);

        // --- SAUVEGARDE APRÈS SUPPRESSION ---
        FlashcardService.SaveFlashcards();
    }
}