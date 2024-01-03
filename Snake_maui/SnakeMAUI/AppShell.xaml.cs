using SnakeModel.Model;
using SnakeModel.Persistence;
using SnakeMAUI.ViewModel;
using SnakeMAUI.View;
namespace SnakeMAUI;

public partial class AppShell : Shell
{
    private ISnakeDataAccess _snakeDataAccess;
    private readonly SnakeGameModel _gameModel;
    private readonly SnakeViewModel _viewModel;

    private readonly IDispatcherTimer _timer;

    private readonly IStore _store;
    private readonly StoredGameBrowserModel _storedGameBrowserModel;
    private readonly StoredGameBrowserViewModel _storedGameBrowserViewModel;

    public AppShell(IStore snakeStore,
        ISnakeDataAccess snakeDataAccess,
        SnakeGameModel snakeGameModel,
        SnakeViewModel snakeViewModel)
    {
        InitializeComponent();
        _store = snakeStore;
        _snakeDataAccess = snakeDataAccess;
        _gameModel = snakeGameModel;
        _viewModel = snakeViewModel;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => _viewModel.StepGame();

        _gameModel.OnGameOver += new EventHandler<GameEventArgs>(Model_GameOver);

        _viewModel.StartTimer += new EventHandler(ViewModel_Start_Timer);
        _viewModel.StopTimer += new EventHandler(ViewModel_Stop_Timer);
        _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
        _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
        _viewModel.ExitGame += new EventHandler(SnakeViewModel_ExitGame);
        
        _storedGameBrowserModel = new StoredGameBrowserModel(_store);
        _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
        _storedGameBrowserViewModel.GameLoading += (StoredGameBrowserViewModel_GameLoading);
        _storedGameBrowserViewModel.GameSaving += (StoredGameBrowserViewModel_GameSaving);
    }

    private async void ViewModel_SaveGame(object? sender, EventArgs e)
    {
        await _storedGameBrowserModel.UpdateAsync();
        await Navigation.PushAsync(new SaveGamePage
        {
            BindingContext = _storedGameBrowserViewModel
        });
    }

    private async void ViewModel_LoadGame(object? sender, EventArgs e)
    {
        await _storedGameBrowserModel.UpdateAsync();
        await Navigation.PushAsync(new LoadGamePage
        {
            BindingContext = _storedGameBrowserViewModel
        });
    }

    private void ViewModel_Stop_Timer(object? sender, EventArgs e)
    {
        StopTimer();
    }

    private void ViewModel_Start_Timer(object? sender, EventArgs e)
    {
        StartTimer();
    }


    internal void StartTimer(){ _timer.Start(); }
    internal void StopTimer() {  _timer.Stop(); }
    private void Model_GameOver(object? sender, GameEventArgs e)
    {
        StopTimer();
        DisplayAlert("Játék vége", $"{e.gameovertext}\nElfogyasztott tojások száma: {e.score}", "OK");
        _viewModel.newGameInModel(_viewModel.Size);
    }

    private async void SnakeViewModel_ExitGame(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage
        {
            BindingContext = _viewModel
        });
    }


    private async void StoredGameBrowserViewModel_GameLoading(object? sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync();

        try
        {
            await _gameModel.LoadGameAsync(e.Name);

            await Navigation.PopAsync();
            await DisplayAlert("Snake", "Sikeres betöltés.", "OK");

            StartTimer();
        }
        catch
        {
            await DisplayAlert("Snake", "Sikertelen betöltés.", "OK");
        }
    }
    

    private async void StoredGameBrowserViewModel_GameSaving(object? sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync();
        StopTimer();

        try
        {
            await _gameModel.SaveGameAsync(e.Name);
            await DisplayAlert("Snake", "Sikeres mentés.", "OK");
        }
        catch
        {
            await DisplayAlert("Snake", "Sikertelen mentés.", "OK");
        }
    }
}
