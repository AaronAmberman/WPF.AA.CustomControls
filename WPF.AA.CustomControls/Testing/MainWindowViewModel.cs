using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Testing
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); ;
        }

        private int value;
        public int Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(); }
        }
    }
}
