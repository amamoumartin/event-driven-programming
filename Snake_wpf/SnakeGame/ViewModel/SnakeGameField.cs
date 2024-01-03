using System;
using System.Windows.Media;
using SnakeModel.Model;

namespace SnakeGame.ViewModel
{
    public class SnakeGameField : ViewModelBase
    {
        private Brush? _color;
        public int X { get; set; }
        public int Y { get; set; }
        public int index { get; set; }
        public Brush? Color { get => _color;
            set { 
                if(_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            } 
        }

        
    }
}
