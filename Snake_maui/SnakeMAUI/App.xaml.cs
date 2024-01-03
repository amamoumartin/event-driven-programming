using SnakeMAUI.ViewModel;
using SnakeModel.Model;
using SnakeModel.Persistence;
namespace SnakeMAUI;

public partial class App : Application
{
    private const string SuspendedGameSavePath = "SuspendedGame";
    
    private readonly AppShell _appShell;
    private readonly ISnakeDataAccess _snakeDataAccess;
    private readonly SnakeGameModel _snakeGameModel;
    private readonly IStore _snakeStore;
    private readonly SnakeViewModel _snakeViewModel;
    public App()
	{
		InitializeComponent();
        _snakeStore = new SnakeStore();
        _snakeDataAccess = new SnakeDataAccess(FileSystem.AppDataDirectory);
        
        _snakeGameModel = new SnakeGameModel(_snakeDataAccess);
        _snakeViewModel = new SnakeViewModel(_snakeGameModel);
        _appShell = new AppShell(_snakeStore,_snakeDataAccess,_snakeGameModel, _snakeViewModel)
        {
            BindingContext = _snakeViewModel
        };
        MainPage = _appShell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += (sender, e) =>
        {
            _snakeViewModel.init(15);
            _appShell.StopTimer();
        };

        window.Activated += (sender, e) =>
        {
            if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                return;

            Task.Run(async () =>
            {
                try
                {
                    await _snakeGameModel.LoadGameAsync(SuspendedGameSavePath);
                    _appShell.StartTimer();
                }
                catch
                {
                }
            });
        };
        
        window.Stopped += (s, e) =>
        {
            Task.Run(async () =>
            {
                try
                {
                    _appShell.StopTimer();
                    await _snakeGameModel.SaveGameAsync(SuspendedGameSavePath);
                }
                catch
                {
                }
            });
        };

        return window;
    }
}
