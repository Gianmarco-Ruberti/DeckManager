using DeckManager.Models;
using DeckManager.Services;
using DeckManager.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace DeckManager;

[QueryProperty(nameof(SelectedDeck), "SelectedDeck")]
public partial class DeckDetailsPage : ContentPage
{
    private GestionViewModel _viewModel;
    private Deck _selectedDeck;
    public Deck SelectedDeck
    {
        get => _selectedDeck;
        set
        {
            _selectedDeck = value;
            Title = $"Deck: {_selectedDeck.Name}";
            LoadFlashcards();
        }
    }

    public ObservableCollection<FlashcardModel> DeckFlashcards { get; } = new();
    public FlashcardModel NewFlashcard { get; set; } = new();

    public DeckDetailsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
    private async void OnEditFlashcard(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FlashcardModel flashcard)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedCard", flashcard }
            };

            await Shell.Current.GoToAsync(nameof(EditPage), navigationParameter);
        }
    }
    private void OnDeleteFlashcard(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FlashcardModel flashcard)
        {
            DeckFlashcards.Remove(flashcard);
            FlashcardService.Delete(flashcard);
            FlashcardService.SaveFlashcards();
        }
    }
    private void LoadFlashcards()
    {
        DeckFlashcards.Clear();
        var flashcards = FlashcardService.GetFlashcardsByDeck(_selectedDeck.Id);
        foreach (var card in flashcards)
        {
            DeckFlashcards.Add(card);
        }
    }

    private void OnAddFlashcardClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewFlashcard.Question) || string.IsNullOrWhiteSpace(NewFlashcard.Answer))
        {
            DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        NewFlashcard.DeckId = _selectedDeck.Id;
        FlashcardService.Add(NewFlashcard);
        FlashcardService.SaveFlashcards();
        DeckFlashcards.Add(NewFlashcard);
        NewFlashcard = new FlashcardModel();
    }
}