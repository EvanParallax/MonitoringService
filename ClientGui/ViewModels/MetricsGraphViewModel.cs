using Common;
using MVVM;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ClientGui.ViewModels
{
    class MetricsGraphViewModel : INotifyPropertyChanged
    {
        private string currentSensor;
        public string CurrentSensor
        {
            get { return currentSensor; }
            set { currentSensor = value; OnPropertyChanged("CurrentSensor"); }
        }

        public List<string> Sensors { get; set; }

        private List<MetricDTO> metrics;
        public List<MetricDTO> Metrics
        {
            get { return metrics; }
            set { metrics = value; OnPropertyChanged("Metrics"); }
        }


        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private RelayCommand metricsChangedComand;

        public RelayCommand MetricsChangedCommand
        {
            get
            {
                return metricsChangedComand ??
                  (metricsChangedComand = new RelayCommand(obj =>
                  {
                      SetUpModel();
                  }));
            }
        }

        public MetricsGraphViewModel(List<MetricDTO> data)
        {
            Metrics = data;

            Sensors = new List<string>();
            foreach (var item in Metrics)
                Sensors.Add(item.SId);
            Sensors = Sensors.Distinct().ToList();
            if(!Metrics.Any())
                currentSensor = Metrics.FirstOrDefault().SId;

            PlotModel = new PlotModel();
            var dateAxis = new DateTimeAxis() { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis() { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetUpModel()
        {
            plotModel.Series.Clear();
            var series = new LineSeries { Title = CurrentSensor, MarkerType = MarkerType.Circle };
            var metrics = Metrics.Where(m => m.SId == CurrentSensor).ToList();
            metrics.Sort();
            foreach (var item in metrics)
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(item.Session), item.Svalue));
            plotModel.Title = currentSensor;
            plotModel.Series.Add(series);
            plotModel.InvalidatePlot(true);
        }
    }
}
