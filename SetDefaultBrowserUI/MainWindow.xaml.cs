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
using SetDefaultBrowserUI.Services;

namespace SetDefaultBrowserUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SdbWrapper _wrapper;

        public MainWindow()
        {
            InitializeComponent();
            _wrapper = new SdbWrapper();

            
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await _wrapper.SetBrowser("chrome");
        }
    }
}
