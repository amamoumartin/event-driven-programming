using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SnakeModel.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;

namespace SnakeGame.ViewModel
{
    public class SnakeViewModel : ViewModelBase
    {
        private SnakeGameModel _model;
        
        public DelegateCommand New10x10GameCommand { get; set; }
        public DelegateCommand New15x15GameCommand { get; set; }
        public DelegateCommand New20x20GameCommand { get; set; }
        
        public DelegateCommand ExitGameCommand { get; set; }

        public ObservableCollection<SnakeGameField> Fields { get; set; }
        public int GameScore{ get { return _model.Score; } }
        public int Size { get { return _model.TableSize; } }
        
        public event EventHandler? ExitGame;

        public SnakeViewModel(SnakeGameModel model)
        {
            _model = model;
            New10x10GameCommand = new DelegateCommand(param => OnNew10x10Game());
            New15x15GameCommand = new DelegateCommand(param => OnNew15x15Game());
            New20x20GameCommand = new DelegateCommand(param => OnNew20x20Game());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());
            init();
        }

        public void newGameInModel(int size)
        {
            _model.StartNewGame(size);
            init();
        }

        public void init()
        {
            generateFields();
            refreshTable();
        }
        public void drawTable()
        {
            OnPropertyChanged("Fields");
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
                    int ind = j * _size + i;
                    switch (_model.Table[i, j])
                    {
                        case Field.Egg:
                            Fields[ind].Color = Brushes.Yellow;
                            break;
                        case Field.Wall:
                            Fields[ind].Color = Brushes.Black;
                            break;
                        case Field.Head:
                            Fields[ind].Color = Brushes.DarkGreen;
                            break;
                        case Field.Body:
                            Fields[ind].Color = Brushes.LightGreen;
                            break;
                        default:
                            Fields[ind].Color = Brushes.LightGray;
                            break;
                    }
                }
            }
            OnPropertyChanged(nameof(Fields));
            
        }

        public void generateFields()
        {
            int _size = _model.TableSize;
            Fields = new ObservableCollection<SnakeGameField>();
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Fields.Add(new SnakeGameField
                    {
                        X = i,
                        Y = j,
                        index = i * _size + j,
                        Color = Brushes.LightGray
                    });
                }
            }
            OnPropertyChanged("Size");
        }

        public void StepGame()
        {
            _model.Move();
            OnPropertyChanged("GameScore");
        }

        private void OnExitGame()
        {
            if(ExitGame != null)
            {
                ExitGame(this, EventArgs.Empty);
            }
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
    }
}
