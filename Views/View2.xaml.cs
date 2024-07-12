using prism_simpletemplate.ViewModels;
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

namespace prism_simpletemplate.Views
{
    /// <summary>
    /// View2.xaml 的交互逻辑
    /// </summary>
    public partial class View2 : UserControl
    {
        public View2(View2ViewModel view2ViewModel)
        {
            this.view2ViewModel = view2ViewModel;
            this.DataContext = view2ViewModel;
            InitializeComponent();
            web.Source = new Uri(System.IO.Directory.GetCurrentDirectory() + "\\Common\\charpage.html");
            web.NavigationCompleted += Web_NavigationCompleted;

            text1.KeyDown += Keyenterdown;
            text2.KeyDown += Keyenterdown;
            text3.KeyDown += Keyenterdown;
            //view2ViewModel.postDelegate = new View2ViewModel.PostDelegate(web.CoreWebView2.PostWebMessageAsJson);
        }
        View2ViewModel view2ViewModel;
        
       
        private void Web_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            view2ViewModel.postDelegate = new View2ViewModel.PostDelegate(web.CoreWebView2.PostWebMessageAsJson);
        }
        void Keyenterdown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as System.Windows.Controls.TextBox;
                textbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                //MessageBox.Show("Enter key is pressed");

            }
        }
    }
}
