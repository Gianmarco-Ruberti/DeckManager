namespace DeckManager;

[QueryProperty(nameof(Time), "Time")]
[QueryProperty(nameof(Difficult), "Difficult")]
[QueryProperty(nameof(Perfect), "Perfect")]
[QueryProperty(nameof(Percent), "Percent")]
public partial class ResumePage : ContentPage
{
    // Variables privées pour stocker temporairement les données reçues
    private string _time = string.Empty;
    private string _difficult = string.Empty;
    private string _perfect = string.Empty;
    private string _percent = string.Empty;

    // Propriétés publiques alimentées par la navigation Shell
    public string Time { set => _time = Uri.UnescapeDataString(value ?? ""); }
    public string Difficult { set => _difficult = Uri.UnescapeDataString(value ?? ""); }
    public string Perfect { set => _perfect = value ?? "0"; }
    public string Percent { set => _percent = value ?? "0"; }

    public ResumePage()
    {
        InitializeComponent();
    }

    // Cette méthode s'exécute quand la page est visible et l'UI est prête
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Sécurité : On applique les textes uniquement maintenant sur les composants XAML
        if (LblTime != null) LblTime.Text = $"Temps passé : {_time}";
        if (LblDifficult != null) LblDifficult.Text = $"Plus difficile : {_difficult}";
        if (LblPerfect != null) LblPerfect.Text = $"Cartes sans faute : {_perfect}";
        if (LblPercent != null) LblPercent.Text = $"{_percent}%";
    }

    private async void OnBackHomeClicked(object sender, EventArgs e)
    {
        // Retour à la page d'accueil (pensez à vérifier que la route est bien "//MainPage" dans votre AppShell.xaml)
        await Shell.Current.GoToAsync("//MainPage");
    }
}