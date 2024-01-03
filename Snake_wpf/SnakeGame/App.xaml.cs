using SnakeGame.View;
using SnakeGame.ViewModel;
using SnakeModel.Model;
using SnakeModel.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SnakeGameModel? _model = null!;
        private SnakeViewModel? _viewModel;
        private MainWindow? _view;
        private DispatcherTimer? _timer;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {

            _model = new SnakeGameModel(new DataAccess());
            _model.StartNewGame(15);
            _viewModel = new SnakeViewModel(_model);
            //_viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new CancelEventHandler(View_Closing);
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += new EventHandler(Timer_Tick);

            _model.OnGameOver += new EventHandler<GameEventArgs>(Model_GameOver);
            _model.DataToDraw += new EventHandler<GameEventArgs>(Model_DrawOnTable);
            _model.ScoreWriter += new EventHandler<GameEventArgs>(Model_DrawScore);

            _view.KeyDown += new KeyEventHandler(KeyPressed);


        }


        private void ViewModel_ExitGame(object? sender, EventArgs e)
        {
            _view!.Close();
        }

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            _timer!.Stop();
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
                _timer.Start();
            }

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _viewModel!.StepGame();
        }

        private void KeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                _model!.SetDirection(Direction.Down);
            }
            else if (e.Key == Key.Up || e.Key == Key.W)
            {
                _model!.SetDirection(Direction.Up);
            }
            else if (e.Key == Key.Left || e.Key == Key.A)
            {
                _model!.SetDirection(Direction.Left);
            }
            else if (e.Key == Key.Right || e.Key == Key.D)
            {
                _model!.SetDirection(Direction.Right);
            }
            else if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                ToggleTimerState();
                _model!.ToggleGameState(_timer!.IsEnabled);
            }
            e.Handled = true;
        }

        private void ToggleTimerState()
        {
            if (_timer!.IsEnabled) _timer.Stop();
            else _timer.Start();
        }

        private void Model_SetTimerIntervalLower(object? sender, GameEventArgs e)
        {
            _timer!.Interval = TimeSpan.FromSeconds(0.5);
        }

        private void Model_DrawScore(object? sender, GameEventArgs e)
        {
            _viewModel!.drawScore();
        }

        private void Model_DrawOnTable(object? sender, GameEventArgs e)
        {
            _viewModel!.Fields[e.y * _model!.TableSize + e.x].Color = e.color;
        }

        private void Model_GameOver(object? sender, GameEventArgs e)
        {
            _timer!.Stop();
            MessageBox.Show($"{e.gameovertext}\nNumber of eggs eaten: {e.score}");
            _viewModel!.newGameInModel(_viewModel.Size);
        }
    }
}
