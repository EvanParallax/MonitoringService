using Common;
using MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientGui
{
    class MainViewModel : INotifyPropertyChanged
    {
        private HttpClient client = new HttpClient();

        private Agent selectedAgent;

        public ObservableCollection<Agent> Agents { get; set; }

        public Agent SelectedAgent
        {
            get { return selectedAgent; }
            set
            {
                selectedAgent = value;
                OnPropertyChanged("SelectedAgent");
            }
        }

        public MainViewModel()
        {
            Agents = new ObservableCollection<Agent>();
            AgentsChanged += MainViewModel_AgentsChanged;
            MetricsRequest += MainViewModel_MetricsRequest;
            AgentsChanged?.Invoke(this, EventArgs.Empty);
            
        }

        private RelayCommand getMetrics;

        public RelayCommand GetMetrics
        {
            get
            {
                return getMetrics ??
                  (getMetrics = new RelayCommand(obj =>
                  {
                      MetricsRequest?.Invoke(this, EventArgs.Empty);
                  }));
            }
        }

        private RelayCommand addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      NewAgentWindow window = new NewAgentWindow();
                      window.Show();
                  }));
            }
        }

        public object Agentgrid { get; private set; }

        private event EventHandler MetricsRequest;

        private async void MainViewModel_MetricsRequest(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/Server/" + selectedAgent.Id.ToString());
            string responseBody = response.Content.ReadAsStringAsync().Result;
            selectedAgent.Metrics = JsonConvert.DeserializeObject<List<MetricDTO>>(responseBody);
        }

        private event EventHandler AgentsChanged;

        private async void MainViewModel_AgentsChanged(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/Server");
            string responseBody = response.Content.ReadAsStringAsync().Result;
            foreach (var item in JsonConvert.DeserializeObject<List<AgentDTO>>(responseBody))
                Agents.Add(new Agent()
                {
                    Id = item.Id,
                    Endpoint = item.Endpoint,
                    OsType = item.OsType,
                    AgentVersion = item.AgentVersion
                });
            SelectedAgent = Agents.First();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

