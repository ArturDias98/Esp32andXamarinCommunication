using BluetoothApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new SelectDeviceView());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
