using Prism.Ioc;
using prism_simpletemplate.Views;
using System.Windows;
using prism_simpletemplate.ViewModels;
using Prism.Regions;
using prism_serial.Views;
using prism_serial.ViewModels;
using System.Text.Json;
using System.IO.Ports;

namespace prism_simpletemplate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<View1, View1ViewModel>();
            containerRegistry.RegisterForNavigation<View2, View2ViewModel>();
            containerRegistry.RegisterForNavigation<View3, View3ViewModel>();
            containerRegistry.RegisterSingleton< SerialPort> ();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", "View1");

        }
    }
}
