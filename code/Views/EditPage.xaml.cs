using DeckManager.Models;
using DeckManager.Services;

namespace DeckManager;

[QueryProperty(nameof(CardToEdit), "SelectedCard")]
public partial class EditPage : ContentPage
{
    private FlashcardModel _cardToEdit = new FlashcardModel();

    public FlashcardModel CardToEdit
    {
        get => _cardToEdit;
        set
        {
            _cardToEdit = value;
            BindingContext = _cardToEdit;
        }
    }

    public EditPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_cardToEdit == null)
            return;

        if (string.IsNullOrWhiteSpace(_cardToEdit.Question) ||
            string.IsNullOrWhiteSpace(_cardToEdit.Answer))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
            return;
        }

        FlashcardService.SaveFlashcards();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
