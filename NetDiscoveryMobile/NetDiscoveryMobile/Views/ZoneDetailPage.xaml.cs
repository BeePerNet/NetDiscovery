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
    public partial class ZoneDetailPage : ContentPage
    {
        DiscoveryZone viewModel;

        public ZoneDetailPage(DiscoveryZone viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ZoneDetailPage()
        {
            InitializeComponent();

        }

        async private void OnZoneItemSelected(object sender, EventArgs e)
        {
            var layout = (Element)sender;

            var item = (DiscoveryZone)layout.BindingContext;

            if (layout.Parent is SelectableItemsView svi)
                svi.SelectedItem = item;

            await Navigation.PushAsync(new ZoneDetailPage(item));
        }

        async private void OnRecordItemSelected(object sender, EventArgs e)
        {
            var layout = (Element)sender;

            var item = (DiscoveryRecord)layout.BindingContext;

            if (layout.Parent is SelectableItemsView svi)
                svi.SelectedItem = item;

            await Navigation.PushAsync(new RecordDetailPage(item));
        }
    }
}