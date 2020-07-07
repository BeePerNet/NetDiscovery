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
    public partial class ZoneHierarchyView : ContentView
    {

        public static BindableProperty IsExpandedProperty = BindableProperty.Create(
                  propertyName: nameof(IsExpanded),
                  returnType: typeof(bool),
                  declaringType: typeof(ZoneHierarchyView),
                  defaultValue: false,
                  defaultBindingMode: BindingMode.TwoWay);

        public bool IsExpanded
        {
            get
            {
                return (bool)base.GetValue(IsExpandedProperty);
            }
            set
            {
                if (this.IsExpanded != value)
                    base.SetValue(IsExpandedProperty, value);
            }
        }
        public ZoneHierarchyView()
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
    }
}