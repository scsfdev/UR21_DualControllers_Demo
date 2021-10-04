using GalaSoft.MvvmLight.Messaging;
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
using UR21_DualControllers_Demo.Model;

namespace UR21_DualControllers_Demo.View
{
    /// <summary>
    /// Interaction logic for Controller.xaml
    /// </summary>
    public partial class Controller : UserControl
    {
        public Controller()
        {
            Messenger.Default.Register<bool>(this, MsgType.CONTROLLER_VMV, ShowHideMemory);

            InitializeComponent();

            MemoryCol.Visibility = Visibility.Collapsed;
        }

        private void ShowHideMemory(bool visible)
        {
            if (visible)
                MemoryCol.Visibility = Visibility.Visible;
            else
                MemoryCol.Visibility = Visibility.Collapsed;
        }
    }
}
