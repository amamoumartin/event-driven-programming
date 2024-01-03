using System;




namespace SnakeMAUI.ViewModel
{
    public class SnakeGameField : ViewModelBase
    {
        private string? _color;
        public int X { get; set; }
        public int Y { get; set; }
        public int index { get; set; }
        public string? Color { get => _color;
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
