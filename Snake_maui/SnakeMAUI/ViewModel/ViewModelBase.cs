using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace SnakeMAUI.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase() { }
#nullable enable
        public event PropertyChangedEventHandler? PropertyChanged;
#nullable disable
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
