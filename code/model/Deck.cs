using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckManager.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int CardCount { get; set; }
        public List<FlashcardModel> Flashcards { get; set; } = new();

        public Deck()
        {
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Name} ({CardCount} cards)";
        }
    }
}
