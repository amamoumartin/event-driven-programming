using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SnakeModel.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.ComponentModel;


namespace SnakeMAUI.ViewModel
{
    public class SnakeViewModel : ViewModelBase
    {
        private SnakeGameModel _model;

        public event EventHandler? StopTimer;
        public event EventHandler? StartTimer;

        private bool _gamestate;

        public DelegateCommand New10x10GameCommand { get; set; }
        public DelegateCommand New15x15GameCommand { get; set; }
        public DelegateCommand New20x20GameCommand { get; set; }

        public DelegateCommand DirectionCommandUp { get; set; }
        public DelegateCommand DirectionCommandDown { get; set; }
        public DelegateCommand DirectionCommandLeft { get; set; }
        public DelegateCommand DirectionCommandRight { get; set; }

        public DelegateCommand LoadGameCommand { get; set; }
        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand ExitGameCommand { get; set; }

        public DelegateCommand StateChange { get; set; }

        public ObservableCollection<SnakeGameField>? Fields { get; set; }
        public int GameScore { get { return _model.Score; } }
        public int Size { get { return _model.TableSize; } }

        public string State { get { return (_gamestate ? "⏸" : "▶"); } }

        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? ExitGame;

        public RowDefinitionCollection GameTableRows
        {
            get => new RowDefinitionCollection(Enumerable.Repeat(new RowDefinition(GridLength.Star), Size).ToArray());
        }

        public ColumnDefinitionCollection GameTableColumns
        {
            get => new ColumnDefinitionCollection(Enumerable.Repeat(new ColumnDefinition(GridLength.Star), Size).ToArray());
        }

        public SnakeViewModel(SnakeGameModel model)
        {
            _model = model;

            Fields = new ObservableCollection<SnakeGameField>();

            _model.DataToDraw += new EventHandler<GameEventArgs>(Model_DrawOnTable);
            _model.ScoreWriter += new EventHandler<GameEventArgs>(Model_DrawScore);

            New10x10GameCommand = new DelegateCommand(param => OnNew10x10Game());
            New15x15GameCommand = new DelegateCommand(param => OnNew15x15Game());
            New20x20GameCommand = new DelegateCommand(param => OnNew20x20Game());

            DirectionCommandUp = new DelegateCommand(param => TurnUp());
            DirectionCommandDown = new DelegateCommand(param => TurnDown());
            DirectionCommandLeft = new DelegateCommand(param => TurnLeft());
            DirectionCommandRight = new DelegateCommand(param => TurnRight());

            StateChange = new DelegateCommand(param => ChangeGameState());


            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());

        }

        private void ChangeGameState()
        {
            _gamestate = !_gamestate;
            _model.ToggleGameState(_gamestate);
            if (_gamestate) StartTimer?.Invoke(this, EventArgs.Empty);
            else StopTimer?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(State));
        }

        private void TurnRight()
        {
            _model.SetDirection(Direction.Right);
        }

        private void TurnLeft()
        {
            _model.SetDirection(Direction.Left);
        }

        private void TurnDown()
        {
            _model.SetDirection(Direction.Down);
        }

        private void TurnUp()
        {
            _model.SetDirection(Direction.Up);
        }

        public void newGameInModel(int size)
        {
            init(size);
        }

        public void init(int size)
        {
            _model.StartNewGame(size);
            generateFields();
            refreshTable();
            _gamestate = true;
            ChangeGameState();
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(GameScore));
        }

        
        public void drawScore()
        {
            OnPropertyChanged("GameScore");
        }
        public void refreshTable()
        {
            int _size = _model.TableSize;
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    int ind = i * _size + j;
                    switch (_model.Table[i, j])
                    {
                        case Field.Egg:
                            Fields![ind].Color = "Yellow";
                            break;
                        case Field.Wall:
                            Fields![ind].Color = "Black";
                            break;
                        case Field.Head:
                            Fields![ind].Color = "DarkGreen";
                            break;
                        case Field.Body:
                            Fields![ind].Color = "LightGreen";
                            break;
                        default:
                            Fields![ind].Color = "LightGray";
                            break;
                    }
                }
            }
            OnPropertyChanged(nameof(Fields));
        }

        public void generateFields()
        {
            int _size = _model.TableSize;
            OnPropertyChanged(nameof(Size));
            OnPropertyChanged(nameof(GameTableColumns));
            OnPropertyChanged(nameof(GameTableRows));
            Fields!.Clear();
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Fields.Add(new SnakeGameField
                    {
                        X = i,
                        Y = j,
                        index = i * _size + j,
                        Color = "LightGray"
                    });
                }
            }
        }

        public void StepGame()
        {
            _model.Move();
            OnPropertyChanged("GameScore");
            OnPropertyChanged(nameof(Fields));
        }

        private void OnLoadGame()
        {
            if (LoadGame != null)
            {
                LoadGame(this, EventArgs.Empty);
            }
        }
        private void OnSaveGame()
        {
            if (SaveGame != null)
            {
                SaveGame(this, EventArgs.Empty);
            }
        }
        
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
            /*
            if (ExitGame != null)
            {
                ExitGame(this, EventArgs.Empty);
            }
            */
        }

        private void OnNew10x10Game()
        {
            newGameInModel(10);
        }

        private void OnNew15x15Game()
        {
            newGameInModel(15);
        }

        private void OnNew20x20Game()
        {
            newGameInModel(20);
        }

        
        private void Model_DrawOnTable(object? sender, GameEventArgs e)
        {
            refreshTable();
        }

        private void Model_DrawScore(object? sender, GameEventArgs e)
        {
            drawScore();
        }
    }
}
