using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPF.AA.CustomControls;

namespace Testing
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); ;
        }

        private decimal value;
        public decimal Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Atom> atoms = new ObservableCollection<Atom>
        {
            new Atom
            {
                Attraction = 0.87,
                Radiation = 0.43,
                Name = "My Atom",
                NumericUpDownType = NumericUpDownType.Double
            }
        };

        public ObservableCollection<Atom> Atoms
        {
            get => atoms;
            set
            {
                atoms = value;
                OnPropertyChanged();
            }
        }
    }

    public class Atom : INotifyPropertyChanged
    {
        private double attraction;
        private double radiation;
        private string name;
        private NumericUpDownType numericUpDownType = NumericUpDownType.Double;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); ;
        }

        public double Attraction
        {
            get => attraction;
            set
            {
                attraction = value;
                OnPropertyChanged();
            }
        }

        public double Radiation
        {
            get => radiation;
            set
            {
                radiation = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public NumericUpDownType NumericUpDownType
        {
            get => numericUpDownType;
            set
            {
                numericUpDownType = value;
                OnPropertyChanged();
            }
        }
    }
}
