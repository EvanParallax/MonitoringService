using ClientGui.ViewModels;
using Common;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientGui.Pages
{
    /// <summary>
    /// Логика взаимодействия для MetricsGraph.xaml
    /// </summary>
    public partial class MetricsGraph : Window
    {
        public MetricsGraph(List<MetricDTO> data)
        {
            MetricsGraphViewModel model = new MetricsGraphViewModel(data);

            DataContext = model;

            InitializeComponent();
        }

        private void Sensors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vievModel =  (MetricsGraphViewModel)(this.DataContext);
            vievModel.SetUpModel();
        }
    }
}
