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
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class RecordDetailPage : ContentPage
    {
        DiscoveryRecord viewModel;

        public RecordDetailPage(DiscoveryRecord viewModel)
        {
            BindingContext = this.viewModel = viewModel;

            InitializeComponent();
        }

        public RecordDetailPage()
        {
            InitializeComponent();

            //viewModel = new ItemDetailViewModel(item);
            //BindingContext = viewModel;
        }

    }
}