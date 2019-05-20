using ClientGui.Pages;
using Common;
using MVVM;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ClientGui
{
    class MainViewModel : INotifyPropertyChanged, IDisposable
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
            EnableAgent += MainViewModel_EnableAgent;
            DisableAgent += MainViewModel_DisableAgent;
            DeleteAgent += MainViewModel_DeleteAgent;
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

        private RelayCommand metricsGrtaphComand;

        public RelayCommand MetricsGraphCommand
        {
            get
            {
                return metricsGrtaphComand ??
                  (metricsGrtaphComand = new RelayCommand(obj =>
                  {
                      if (SelectedAgent.Metrics != null)
                      {
                          MetricsGraph window = new MetricsGraph(SelectedAgent.Metrics);
                          window.Show();
                      }
                      else
                          MessageBox.Show("There is no metrics, push 'Get metrics' button to load metrics");
                  }));
            }
        }

        private RelayCommand enableCommand;

        public RelayCommand EnableCommand
        {
            get
            {
                return enableCommand ??
                  (enableCommand = new RelayCommand(obj =>
                  {
                          EnableAgent?.Invoke(this, EventArgs.Empty);
                  }));
            }
        }

        private RelayCommand disableCommand;

        public RelayCommand DisableCommand
        {
            get
            {
                return disableCommand ??
                  (disableCommand = new RelayCommand(obj =>
                  {
                      EnableAgent?.Invoke(this, EventArgs.Empty);
                  }));
            }
        }

        private RelayCommand deleteCommand;

        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      DeleteAgent?.Invoke(this, EventArgs.Empty);
                  }));
            }
        }

        private event EventHandler EnableAgent;

        private async void MainViewModel_EnableAgent(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/agent/enableagent/" + selectedAgent.Id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }

            
        }

        private event EventHandler DisableAgent;

        private async void MainViewModel_DisableAgent(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/agent/disableagent/" + selectedAgent.Id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }
        }

        private event EventHandler DeleteAgent;

        private async void MainViewModel_DeleteAgent(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/agent/deleteagent/" + selectedAgent.Id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }

        }

        public object Agentgrid { get; private set; }

        private event EventHandler MetricsRequest;

        private async void MainViewModel_MetricsRequest(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/Metrics/" + selectedAgent.Id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            selectedAgent.Metrics = JsonConvert.DeserializeObject<MetricsList>(responseBody).Metrics;
        }

        private event EventHandler AgentsChanged;

        private async void MainViewModel_AgentsChanged(object sender, EventArgs e)
        {
            var response = await client.GetAsync("Http://localhost:59217/api/Agent");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
                return;
            }
                
            string responseBody = await response.Content.ReadAsStringAsync();

            foreach (var item in JsonConvert.DeserializeObject<AgentsList>(responseBody).Agents)
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

        public void Dispose()
        {
            client.Dispose();
        }
    }
}

