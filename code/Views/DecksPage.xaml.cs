using System.Collections.ObjectModel;
using DeckManager.Models;
using DeckManager.Services;

namespace DeckManager
{
    public partial class DecksPage : ContentPage
    {
        private JsonDataService _dataService;
        private ObservableCollection<Deck> _decks;
        private int _nextId = 1;

        public DecksPage()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _decks = new ObservableCollection<Deck>();
            LoadDecks();
        }

        private async void LoadDecks()
        {
            var loadedDecks = await _dataService.LoadDecksAsync();

            _decks.Clear();
            foreach (var deck in loadedDecks)
            {
                _decks.Add(deck);
            }

            if (_decks.Any())
            {
                _nextId = _decks.Max(d => d.Id) + 1;
            }

            if (DecksCollectionView.ItemsSource == null)
            {
                DecksCollectionView.ItemsSource = _decks;
            }

            UpdateInfo($"Chargé: {_decks.Count} deck(s)");
        }

        private async void OnDeckTapped(object sender, EventArgs e)
        {
            var layout = (BindableObject)sender;
            var deck = layout.BindingContext as Deck;

            if (deck == null) return;

            await Shell.Current.GoToAsync($"DeckDetailsPage", true, new Dictionary<string, object>
            {
                { "SelectedDeck", deck }
            });
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                DecksCollectionView.ItemsSource = _decks;
                return;
            }

            var filtered = _decks.Where(d => (d.Name ?? string.Empty).ToLower().Contains(searchText));
            DecksCollectionView.ItemsSource = new ObservableCollection<Deck>(filtered);
        }

        private void RefreshView()
        {
            // Force le rafraîchissement de l'UI
            DecksCollectionView.ItemsSource = null;
            DecksCollectionView.ItemsSource = _decks;
        }

        private void UpdateInfo(string message)
        {
            InfoLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }

        private async void OnAddDeckClicked(object sender, EventArgs e)
        {
            string? name = NewDeckEntry.Text?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un nom", "OK");
                return;
            }

            Deck newDeck = new Deck
            {
                Id = _nextId++,
                Name = name,
                CardCount = 0
            };

            _decks.Add(newDeck);
            await _dataService.SaveDecksAsync(_decks.ToList());

            NewDeckEntry.Text = string.Empty;
            UpdateInfo($"Ajouté: {name}");
        }

        // --- ÉDITION INLINE ---

        private void OnEditDeckInlineClicked(object sender, EventArgs e)
        {
            ToggleEditMode(sender as Button, true);
        }

        private async void OnSaveEditClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var deck = button?.CommandParameter as Deck;
            if (deck == null) return;

            var parentLayout = button?.Parent as HorizontalStackLayout;
            var entry = parentLayout?.Children.OfType<Entry>().FirstOrDefault();

            if (entry != null)
            {
                string newName = entry.Text?.Trim() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    deck.Name = newName;
                    await _dataService.SaveDecksAsync(_decks.ToList());
                    UpdateInfo($"Modifié: {newName}");

                    // Indispensable pour voir la modif sans redémarrer si INotifyPropertyChanged n'est pas utilisé
                    RefreshView();
                }
            }

            ToggleEditMode(button, false);
        }

        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            ToggleEditMode(sender as Button, false);
        }

        private void ToggleEditMode(Button? button, bool isEditing)
        {
            if (button == null) return;

            // Remonte au Grid parent pour trouver les stacks de lecture et d'écriture
            Element? current = button;
            while (current != null && !(current is Grid))
            {
                current = current.Parent;
            }

            if (current is Grid grid)
            {
                var displayStack = grid.Children.OfType<StackLayout>().FirstOrDefault();
                var editStack = grid.Children.OfType<HorizontalStackLayout>().FirstOrDefault();

                if (displayStack != null && editStack != null)
                {
                    displayStack.IsVisible = !isEditing;
                    editStack.IsVisible = isEditing;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshView();
        }

        private async void OnDeleteDeckClicked(object sender, EventArgs e)
        {
            var deck = (sender as Button)?.CommandParameter as Deck;
            if (deck == null) return;

            bool confirm = await DisplayAlert("Confirmation", $"Supprimer '{deck.Name}' ?", "Supprimer", "Annuler");
            if (!confirm) return;

            _decks.Remove(deck);
            await _dataService.SaveDecksAsync(_decks.ToList());
            UpdateInfo($"Supprimé: {deck.Name}");
        }
    }
}