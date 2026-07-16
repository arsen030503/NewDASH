using Avalonia.Controls;
using LiveChartsDashboard.ViewModels;

namespace LiveChartsDashboard
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
