using DeckManager.Services;
namespace DeckManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        FlashcardService.LoadFlashcards();
        Routing.RegisterRoute(nameof(GestionPage), typeof(GestionPage));
        Routing.RegisterRoute(nameof(EditPage), typeof(EditPage));
        Routing.RegisterRoute(nameof(DeckDetailsPage), typeof(DeckDetailsPage));
        Routing.RegisterRoute(nameof(ApprentissagePage), typeof(ApprentissagePage));
        Routing.RegisterRoute(nameof(ResumePage), typeof(ResumePage));
    }
}