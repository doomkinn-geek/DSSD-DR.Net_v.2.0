using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using XRayApp.CrossUI.ViewModels;
using XRayApp.CrossUI.Views;
using XRayApp.Data;

namespace XRayApp.CrossUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DatabaseManager _databaseManager = new DatabaseManager();
                _databaseManager.InitializeDatabase();

                var viewModelLocator = new ViewModelLocator(_databaseManager);  // —оздаем экземпл€р ViewModelLocator
                Resources["Locator"] = viewModelLocator;

                var mainWindow = new MainWindow
                {
                    DataContext = viewModelLocator.MainWindowViewModel,  // »спользуем ViewModelLocator дл€ получени€ ViewModel
                };

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}