using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClientGui
{
    public class Agent : INotifyPropertyChanged
    {
        private Guid id;
        private string endpoint;
        private string osType;
        private string agentVersion;
        private bool isEnabled;
        private List<MetricDTO> metrics;

        public List<MetricDTO> Metrics
        {
            get { return metrics; }

            set
            {
                metrics = value;
                OnPropertyChanged("Metrics");
            }

        }

        public Guid Id
        {
            get { return id; }

            set
            {
                id = value;
                OnPropertyChanged("Id");
            }

        }

        public string Endpoint
        {
            get { return endpoint; }

            set
            {
                endpoint = value;
                OnPropertyChanged("Endpoint");
            }

        }

        public string OsType
        {
            get { return osType; }

            set
            {
                osType = value;
                OnPropertyChanged("OsType");
            }

        }

        public string AgentVersion
        {
            get { return agentVersion; }

            set
            {
                agentVersion = value;
                OnPropertyChanged("AgentVersion");
            }

        }

        public bool IsEnabled
        {
            get { return isEnabled; }

            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
