using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeckManager.Models;
using DeckManager.Services;
using Microsoft.Maui.ApplicationModel;

namespace DeckManager.ViewModels;

public class ApprentissageViewModel : BindableObject
{
    private FlashcardModel? currentFlashcard;
    private int currentIndex = 0;
    private double progress = 0;
    private DateTime startTime;

    private DateTime _cardStartTime;

    // Listes pour gérer la session
    private List<FlashcardModel> _cardsPool = new();
    // Dictionnaire pour suivre les erreurs commises *durant cette session* [Id/Question -> Nombre d'erreurs]
    private Dictionary<string, int> _sessionFailures = new();

    public ObservableCollection<FlashcardModel> Flashcards { get; set; } = new();

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
    }

    public void InitialiserCartes(List<FlashcardModel> cartesDuDeck)
    {
        _cardsPool = cartesDuDeck.ToList();

        // RÈGLE : Lancer les cartes de manière aléatoire
        MelangerCartes();

        Flashcards.Clear();
        foreach (var card in _cardsPool)
        {
            Flashcards.Add(card);
        }

        ReinitialiserSession();
    }

    public void ReinitialiserSession()
    {
        currentIndex = 0;
        Progress = 0;
        _sessionFailures.Clear();
        startTime = DateTime.Now;

        if (Flashcards.Count > 0)
        {
            CurrentFlashcard = Flashcards[0];
            _cardStartTime = DateTime.Now;
        }
        else
        {
            CurrentFlashcard = new FlashcardModel { Question = "Aucune carte trouvée", Answer = "Créez des cartes d'abord !" };
        }
    }

    // Mélange de Fisher-Yates pour garantir un vrai aléatoire
    private void MelangerCartes()
    {
        Random rand = new Random();
        int n = _cardsPool.Count;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            var value = _cardsPool[k];
            _cardsPool[k] = _cardsPool[n];
            _cardsPool[n] = value;
        }
    }

    public void MarkAsSuccess()
    {
        if (CurrentFlashcard != null && currentIndex < Flashcards.Count)
        {
            // Initialise la clé dans le dictionnaire de fautes si elle n'existe pas encore
            // (Utile pour savoir qu'elle a été vue, même s'il y a 0 faute)
            string cle = CurrentFlashcard.Id != 0 ? CurrentFlashcard.Id.ToString() : CurrentFlashcard.Question;
            if (!_sessionFailures.ContainsKey(cle))
            {
                _sessionFailures[cle] = 0;
            }

            CurrentFlashcard.SuccessCount++;
            FlashcardService.SaveFlashcards();
            NextFlashcard();
        }
    }

    public void MarkAsFailed()
    {
        if (CurrentFlashcard != null && currentIndex < Flashcards.Count)
        {
            string cle = CurrentFlashcard.Id != 0 ? CurrentFlashcard.Id.ToString() : CurrentFlashcard.Question;

            // On incrémente le nombre de fautes pour cette session
            if (_sessionFailures.ContainsKey(cle))
                _sessionFailures[cle]++;
            else
                _sessionFailures[cle] = 1;

            CurrentFlashcard.FailureCount++;
            FlashcardService.SaveFlashcards();
            NextFlashcard();
        }
    }

    private void NextFlashcard()
    {
        currentIndex++;
        if (Flashcards.Count == 0) return;

        Progress = (double)(currentIndex % Flashcards.Count) / Flashcards.Count;

        if (currentIndex < Flashcards.Count)
        {
            CurrentFlashcard = Flashcards[currentIndex];
            _cardStartTime = DateTime.Now;
        }
        else
        {
            // RÈGLE : Le mode continue tant que l'utilisateur ne choisit pas de stopper.
            // On boucle à l'infini en remélangeant pour éviter d'avoir le même ordre au second tour !
            MelangerCartes();
            Flashcards.Clear();
            foreach (var card in _cardsPool)
            {
                Flashcards.Add(card);
            }

            currentIndex = 0;
            CurrentFlashcard = Flashcards[currentIndex];
            _cardStartTime = DateTime.Now;
        }
    }

    public void ForcerFinSession()
    {
        SessionTerminee();
    }

    private void SessionTerminee()
    {
        var totalTime = DateTime.Now - startTime;
        string difficultCardName = "Aucune (Sans faute !)";

        // 1. Trouver la carte la plus difficile (le plus d'erreurs commises dans la session)
        var cartesEchouees = _sessionFailures.Where(p => p.Value > 0).ToList();
        if (cartesEchouees.Count > 0)
        {
            var idCarteLaPlusEchouee = cartesEchouees.OrderByDescending(pair => pair.Value).First().Key;
            var carteDifficile = Flashcards.FirstOrDefault(f => f.Id.ToString() == idCarteLaPlusEchouee || f.Question == idCarteLaPlusEchouee);
            if (carteDifficile != null)
            {
                difficultCardName = $"{carteDifficile.Question} ({_sessionFailures[idCarteLaPlusEchouee]}x faux)";
            }
        }

        // 2. RÈGLE : Nombre de cartes connues à 100% (Aucune erreur commise sur TOUTES les cartes du deck pendant cette session)
        // On vérifie parmi toutes les cartes du Deck initial lesquelles n'ont pas d'erreur enregistrée.
        int perfectCardsCount = 0;
        foreach (var card in _cardsPool)
        {
            string cle = card.Id != 0 ? card.Id.ToString() : card.Question;

            // Une carte est parfaite si elle a été vue (présente dans le dictionnaire) ET que son compteur d'erreurs est égal à 0
            if (_sessionFailures.ContainsKey(cle) && _sessionFailures[cle] == 0)
            {
                perfectCardsCount++;
            }
        }

        // 3. RÈGLE : % de mémorisation (Cartes à 100% / Nombre total de cartes du deck)
        double memoPercent = _cardsPool.Count > 0
            ? ((double)perfectCardsCount / _cardsPool.Count) * 100
            : 0;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"{nameof(ResumePage)}?" +
                $"Time={totalTime.Minutes}m {totalTime.Seconds}s&" +
                $"Difficult={Uri.EscapeDataString(difficultCardName)}&" +
                $"Perfect={perfectCardsCount}&" +
                $"Percent={memoPercent:F0}");
        });
    }
}