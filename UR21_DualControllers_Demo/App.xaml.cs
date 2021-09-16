using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace UR21_DualControllers_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(OnTextBoxGotFocus));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.LostFocusEvent, new RoutedEventHandler(OnTextBoxLostFocus));
        }


        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox && !(sender as TextBox).IsReadOnly)
            {
                (sender as TextBox).Foreground = Brushes.Blue;
                (sender as TextBox).Background = (Brush)Current.MainWindow.FindResource("CtrlFocusColor");
                (sender as TextBox).SelectAll();
            }
        }

        private static void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox && !(sender as TextBox).IsReadOnly)
            {
                (sender as TextBox).Text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Background = Brushes.White;
                (sender as TextBox).Foreground = Brushes.Black;
            }
        }
    }
}
