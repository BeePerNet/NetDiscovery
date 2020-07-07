using NetDiscovery.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NetDiscoveryMobile.Views
{
    [DesignTimeVisible(false)]
    public partial class ParametersPage : ContentPage
    {
        DiscoveryClient viewModel;

        public ParametersPage()
        {
            InitializeComponent();

            DiscoveryClient client = (App.Current as App).Client;

            BindingContext = viewModel = client;
        }

    }
}