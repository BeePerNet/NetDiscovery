using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using NetDiscoveryMobile.ViewModels;
using NetDiscovery.Lib;

namespace NetDiscoveryMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PortDetailPage : ContentPage
    {
        DiscoveryPort viewModel;

        public PortDetailPage(DiscoveryPort viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public PortDetailPage()
        {
            InitializeComponent();

        }

        async private void OnInstanceItemSelected(object sender, EventArgs e)
        {
            var layout = (Element)sender;

            var item = (DiscoveryZone)layout.BindingContext;

            if (layout.Parent is SelectableItemsView svi)
                svi.SelectedItem = item;

            await Navigation.PushAsync(new ZoneDetailPage(item));

        }
    }
}