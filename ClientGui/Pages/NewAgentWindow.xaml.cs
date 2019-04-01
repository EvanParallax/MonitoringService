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

namespace ClientGui
{
    /// <summary>
    /// Логика взаимодействия для NewAgentWindow.xaml
    /// </summary>
    public partial class NewAgentWindow : Window
    {
        public NewAgentWindow()
        {
            InitializeComponent();

            DataContext = new NewAgentViewModel();
        }
    }
}
