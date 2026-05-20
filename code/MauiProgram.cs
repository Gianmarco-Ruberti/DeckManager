using Microsoft.Extensions.Logging;
using DeckManager.ViewModels;
using DeckManager.Services;

namespace DeckManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterPages()
                .RegisterViewModels();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<DeckDetailsPage>();
            builder.Services.AddSingleton<ApprentissagePage>();
            builder.Services.AddSingleton<ResumePage>();
            builder.Services.AddSingleton<GestionPage>();
            builder.Services.AddSingleton<DecksPage>();
            builder.Services.AddSingleton<EditDeckPage>();
            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<JsonDataService>();
            builder.Services.AddSingleton<DeckManager.ViewModels.ApprentissageViewModel>();
            builder.Services.AddSingleton<DeckManager.ViewModels.GestionViewModel>();
            return builder;
        }
    }
}
