using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using NetDiscoveryMobile.Views;
using NetDiscoveryMobile.ViewModels;
using NetDiscovery.Lib;

namespace NetDiscoveryMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class InterfacesPage : ContentPage
    {
        DiscoveryClient viewModel;

        public InterfacesPage()
        {
            InitializeComponent();

            DiscoveryClient client = (App.Current as App).Client;

            BindingContext = viewModel = client;
        }


        async private void OnInterfaceItemSelected(object sender, EventArgs e)
        {
            var layout = (Element)sender;

            var item = (DiscoveryInterface)layout.BindingContext;

            if (layout.Parent is SelectableItemsView svi)
                svi.SelectedItem = item;

            await Navigation.PushAsync(new InterfaceDetailPage(item));

        }
    }
}