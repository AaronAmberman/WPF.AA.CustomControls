using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Testing
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); ;
        }

        private Color previousColor = Colors.Red;
        private Color selectedColor = Colors.Blue;

        public Color PreviousColor
        {
            get { return previousColor; }
            set
            {
                if (previousColor != value)
                {
                    previousColor = value;
                    OnPropertyChanged(nameof(PreviousColor));
                }
            }
        }

        public Color SelectedColor
        {
            get { return selectedColor; }
            set
            {
                if (selectedColor != value)
                {
                    selectedColor = value;
                    OnPropertyChanged(nameof(SelectedColor));
                }
            }
        }
    }
}
