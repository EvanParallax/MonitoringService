using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClientGui.Models
{
    class Metrics : INotifyPropertyChanged
    {
        private string currentSensor;
        public string CurrentSensor
        {
            get { return currentSensor; }
            set { currentSensor = value; OnPropertyChanged("CurrentSensor"); }
        }

        public ObservableCollection<string> Sensors { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
