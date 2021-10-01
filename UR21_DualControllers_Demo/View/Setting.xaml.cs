using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using UR21_DualControllers_Demo.Model;

namespace UR21_DualControllers_Demo.View
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void Button_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(groupMain.IsEnabled)
                Messenger.Default.Send(contentBinding.Content.ToString().ToUpper().Replace("CONTROLLER ", "") + "," + (sender as Button).IsEnabled, MsgType.CON_STATUS);
        }
    }
}
