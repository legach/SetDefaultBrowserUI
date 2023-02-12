using System.Windows;
using System.Windows.Controls;
using SetDefaultBrowserUI.Services;
using SetDefaultBrowserUI.ViewModels;

namespace SetDefaultBrowserUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            var wrapper = new SdbWrapper();
            DataContext = new MainViewModel(wrapper);
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu cm && cm.DataContext==null)
            {
                cm.DataContext = DataContext;
            }
        }
    }
}
