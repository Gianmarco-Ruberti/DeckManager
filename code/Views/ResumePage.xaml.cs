namespace DeckManager;

[QueryProperty(nameof(Time), "Time")]
[QueryProperty(nameof(Difficult), "Difficult")]
[QueryProperty(nameof(Perfect), "Perfect")]
[QueryProperty(nameof(Percent), "Percent")]
public partial class ResumePage : ContentPage
{
    // On utilise les noms définis dans le XAML (LblTime, etc.)
    public string Time { set => LblTime.Text = $"Temps passé : {Uri.UnescapeDataString(value)}"; }
    public string Difficult { set => LblDifficult.Text = $"Plus difficile : {Uri.UnescapeDataString(value)}"; }
    public string Perfect { set => LblPerfect.Text = $"Cartes sans faute : {value}"; }
    public string Percent { set => LblPercent.Text = $"{value}%"; }

    public ResumePage()
    {
        InitializeComponent();
    }

    private async void OnBackHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}