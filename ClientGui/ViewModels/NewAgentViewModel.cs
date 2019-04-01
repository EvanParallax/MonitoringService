using MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientGui
{
    public class NewAgentViewModel : INotifyPropertyChanged
    {
        private static HttpClient client = new HttpClient();

        private Agent newAgent;

        public Agent NewAgent
        {
            get { return newAgent; }
            set
            {
                newAgent = value;
                OnPropertyChanged("newAgent");
            }
        }

        public NewAgentViewModel()
        {
            newAgent = new Agent();
            AddingAgent += NewAgentViewModel_AddingAgent;
        }
        

        private RelayCommand addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      AddingAgent?.Invoke(this, EventArgs.Empty);
                  }));
            }
        }

        public event EventHandler AddingAgent;

        private async void NewAgentViewModel_AddingAgent(object sender, EventArgs e)
        {
            var response = await client.PostAsJsonAsync("Http://localhost:59217/api/Server/", newAgent);
            MessageBox.Show("New agent added");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
