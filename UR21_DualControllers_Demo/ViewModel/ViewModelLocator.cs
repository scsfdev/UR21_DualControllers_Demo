/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:UR21_DualControllers_Demo.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using UR21_DualControllers_Demo.Model;

namespace UR21_DualControllers_Demo.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register(() => new SettingViewModel(1),"1");

            //SimpleIoc.Default.Register<ControllerViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ControllerViewModel ControllerVM1
        {
            get
            {
                return new ControllerViewModel(1);
            }
        }

        public ControllerViewModel ControllerVM2
        {
            get
            {
                return new ControllerViewModel(2);
            }
        }

        public SettingViewModel SettingVM_Design
        {
            get
            {
                return new SettingViewModel();
            }
        }

        public SettingViewModel SettingVM1
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingViewModel>("1");
            }
        }

        public SettingViewModel SettingVM2
        {
            get
            {
                return new SettingViewModel(2);
            }
        }


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}