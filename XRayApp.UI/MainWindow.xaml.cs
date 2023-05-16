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
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRayApp.Data;
using XRayApp.UI.ViewModel;

namespace XRayApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            // Set the data context to the view model
            DataContext = viewModel;
        }*/
        public MainWindowViewModel ViewModel
        {
            get { return DataContext as MainWindowViewModel; }
            set { DataContext = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            var databaseManager = new DatabaseManager();
            databaseManager.InitializeDatabase();
        }
    }
}
