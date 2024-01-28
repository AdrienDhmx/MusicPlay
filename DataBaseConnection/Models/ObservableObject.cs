using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicPlay.Database.Models
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value) || propertyName is null) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
