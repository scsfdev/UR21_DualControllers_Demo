using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UR21_DualControllers_Demo.Model;
using UR21_DualControllers_Demo.ViewModel;

namespace UR21_DualControllers_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            Messenger.Default.Register<string>(this, MsgType.MAIN_V, ShowMsg);
            Messenger.Default.Register<NotificationMessage>(this, (nm) =>
            {
                if (nm.Notification == Model.MyConst.EXIT)
                {
                    Close();
                }
            });

            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void TextBlock_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            string strMsg = (sender as TextBlock).Text;

            if (string.IsNullOrEmpty(strMsg))
            {
                (sender as TextBlock).Foreground = Brushes.Black;
                (sender as TextBlock).ClearValue(TextBlock.BackgroundProperty);
            }
            else if (strMsg.ToUpper().StartsWith("ERROR") || strMsg.ToUpper().StartsWith("WARNING"))
            {
                (sender as TextBlock).Foreground = Brushes.White;
                (sender as TextBlock).Background = Brushes.Red;
            }
            else
            {
                (sender as TextBlock).Foreground = Brushes.Black;
                (sender as TextBlock).ClearValue(TextBlock.BackgroundProperty);
            }

            (sender as TextBlock).ToolTip = strMsg;
        }

        private void ShowMsg(string strMsg)
        {
            MessageBoxImage mboxImg;

            if (strMsg.ToUpper().Contains(MyConst.ERROR))
                mboxImg = MessageBoxImage.Error;
            else if (strMsg.ToUpper().Contains(MyConst.WARNING))
                mboxImg = MessageBoxImage.Warning;
            else
                mboxImg = MessageBoxImage.Information;

            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(this, strMsg, MyConst.TITLE, MessageBoxButton.OK, mboxImg);
            });
        }
    }
}