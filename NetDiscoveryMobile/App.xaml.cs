using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NetDiscoveryMobile.Views;
using NetDiscovery.Lib;

namespace NetDiscoveryMobile
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        /*public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
        public static bool UseMockDataStore = true;*/

        public App()
        {
            //Device.SetFlags(new string[] { "SwipeView_Experimental", "SwipeView_Experimental", "AppTheme_Experimental" });
            //Xamarin.Forms.DataGrid.DataGridComponent.Init();

            InitializeComponent();

            /*if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<AzureDataStore>();*/
            //DependencyService.Register<DiscoveryClient>();

            //DependencyService.Get<DiscoveryClient>().Start();

            MainPage = new MainPage();

            Application.Current.UserAppTheme = OSAppTheme.Dark;
        }

        public DiscoveryClient Client { get; } = new DiscoveryClient();

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
