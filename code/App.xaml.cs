namespace DeckManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register navigation routes
            Routing.RegisterRoute("EditDeck", typeof(EditDeckPage));

            MainPage = new AppShell();
        }
    }
}