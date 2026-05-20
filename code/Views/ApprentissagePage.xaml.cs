using DeckManager.ViewModels;
using System.Threading.Tasks;

namespace DeckManager;

public partial class ApprentissagePage : ContentPage
{
    private readonly ApprentissageViewModel _viewModel;
    private bool _isFlipped = false;


    public ApprentissagePage()
    {
        InitializeComponent();
        _viewModel = new ApprentissageViewModel();
        BindingContext = _viewModel;

        // react to card changes to reset flip state
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_viewModel.CurrentFlashcard))
            {
                ResetCardState();
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // On appelle le Reset ici pour que currentIndex repasse 0 
        // chaque fois qu'on lance une session
        if (BindingContext is ApprentissageViewModel vm)
        {
            vm.ReinitialiserSession();
            ResetCardState();
        }

        if (Accelerometer.Default.IsSupported)
        {
            Accelerometer.Default.ShakeDetected += OnShakeDetected;
            Accelerometer.Default.Start(SensorSpeed.Default);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (Accelerometer.Default.IsSupported)
        {
            Accelerometer.Default.Stop();
            Accelerometer.Default.ShakeDetected -= OnShakeDetected;
        }
    }

    private void OnShakeDetected(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (!_isFlipped)
            {
                await DisplayAlert("Info", "Retournez la carte avant d'utiliser le shake.", "OK");
                return;
            }

            _viewModel.MarkAsFailed();
        });
    }

    private async void OnCloseClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private void OnFailedClicked(object sender, EventArgs e)
    {
        if (!_isFlipped)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("Info", "Retournez la carte pour voir la réponse avant de marquer.", "OK"));
            return;
        }

        _viewModel.MarkAsFailed();
    }

    private void OnSuccessClicked(object sender, EventArgs e)
    {
        if (!_isFlipped)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("Info", "Retournez la carte pour voir la réponse avant de marquer.", "OK"));
            return;
        }

        _viewModel.MarkAsSuccess();
    }

    private async void OnCardTapped(object sender, EventArgs e)
    {
        await ToggleCardFlip();
    }

    private async Task ToggleCardFlip()
    {
        var front = CardFront;
        var back = CardBack;
        if (front == null || back == null) return;

        if (!_isFlipped)
        {
            // flip front -> back
            await front.RotateYTo(90, 150);
            front.IsVisible = false;
            back.RotationY = -90;
            back.IsVisible = true;
            await back.RotateYTo(0, 150);
            _isFlipped = true;
        }
        else
        {
            // flip back -> front
            await back.RotateYTo(90, 150);
            back.IsVisible = false;
            front.RotationY = -90;
            front.IsVisible = true;
            await front.RotateYTo(0, 150);
            _isFlipped = false;
        }

        // enable/disable buttons depending on flip state
        var failed = FailedButton;
        var success = SuccessButton;
        if (failed != null) failed.IsEnabled = _isFlipped;
        if (success != null) success.IsEnabled = _isFlipped;
    }

    private void ResetCardState()
    {
        _isFlipped = false;
        var front = CardFront;
        var back = CardBack;
        var failed = FailedButton;
        var success = SuccessButton;
        if (front != null)
        {
            front.IsVisible = true;
            front.RotationY = 0;
        }
        if (back != null)
        {
            back.IsVisible = false;
            back.RotationY = 0;
        }
        if (failed != null) failed.IsEnabled = false;
        if (success != null) success.IsEnabled = false;
    }
}