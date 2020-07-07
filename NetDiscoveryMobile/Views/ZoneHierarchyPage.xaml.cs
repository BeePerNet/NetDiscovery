using NetDiscovery.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NetDiscoveryMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZoneHierarchyPage : ContentPage
    {

        DiscoveryClient viewModel;

        public ZoneHierarchyPage()
        {
            InitializeComponent();

            DiscoveryClient client = (App.Current as App).Client;

            BindingContext = viewModel = client;
        }

    }
}