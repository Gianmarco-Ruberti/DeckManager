using DeckManager.Models;
using DeckManager.ViewModels;

namespace DeckManager;

public partial class GestionPage : ContentPage
{
    private readonly GestionViewModel _viewModel;

    public GestionPage()
    {
        InitializeComponent();
        _viewModel = new GestionViewModel();
        BindingContext = _viewModel;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_viewModel.SaveFlashcard())
            await Shell.Current.GoToAsync("..");
    }
    private async void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var card = button.BindingContext as FlashcardModel;

        if (card != null)
        {
            var navigationParameter = new Dictionary<string, object>
        {
            { "SelectedCard", card }
        };

            // On envoie la carte à l'EditPage
            await Shell.Current.GoToAsync(nameof(EditPage), navigationParameter);
        }
    }
}