using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XRayApp.Data;
using XRayApp.UI.ViewModel;

namespace XRayApp.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            DatabaseManager _databaseManager = new DatabaseManager();
            _databaseManager.InitializeDatabase();

            var viewModelLocator = new ViewModelLocator(_databaseManager);  // Создаем экземпляр ViewModelLocator
            Resources["Locator"] = viewModelLocator;
            MainWindow mainWindow = new MainWindow();
            mainWindow.ViewModel = viewModelLocator.MainWindowViewModel;  // Используем ViewModelLocator для получения ViewModel
            mainWindow.Show();
        }
    }
}
