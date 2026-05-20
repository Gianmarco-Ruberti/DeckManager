using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using DeckManager.Models;

namespace DeckManager.Services;

public static class FlashcardService
{
    private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "flashcards.json");

    public static ObservableCollection<FlashcardModel> Flashcards { get; } = new();

    public static void LoadFlashcards()
    {
        try
        {
            if (!File.Exists(FilePath))
                return;

            string json = File.ReadAllText(FilePath);
            var cards = JsonSerializer.Deserialize<List<FlashcardModel>>(json);
            if (cards == null)
                return;

            Flashcards.Clear();
            foreach (var card in cards)
            {
                Flashcards.Add(card);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to load flashcards: {ex.Message}");
        }
    }

    public static void SaveFlashcards()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(Flashcards.ToList(), options);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to save flashcards: {ex.Message}");
        }
    }

    public static void Add(FlashcardModel card)
    {
        if (card == null)
            return;

        if (card.Id == 0)
        {
            card.Id = Flashcards.Any() ? Flashcards.Max(c => c.Id) + 1 : 1;
        }

        if (!Flashcards.Any(c => c.Id == card.Id))
        {
            Flashcards.Add(card);
        }
    }

    public static void Delete(FlashcardModel card)
    {
        if (card == null)
            return;

        Flashcards.Remove(card);
    }

    public static List<FlashcardModel> GetFlashcardsByDeck(int deckId)
    {
        return Flashcards.Where(card => card.DeckId == deckId).ToList();
    }
}
