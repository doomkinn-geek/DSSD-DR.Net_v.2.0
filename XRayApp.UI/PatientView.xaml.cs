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
using XRayApp.UI.ViewModel;

namespace XRayApp.UI
{
    /// <summary>
    /// Логика взаимодействия для PatientView.xaml
    /// </summary>
    public partial class PatientView : UserControl
    {
        public PatientView()
        {
            InitializeComponent();
            var app = (App)Application.Current;
            var locator = (ViewModelLocator)app.Resources["Locator"];
            DataContext = locator.PatientViewModel;
        }
    }
}
