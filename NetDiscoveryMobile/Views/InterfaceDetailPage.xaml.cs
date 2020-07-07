using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using NetDiscovery.Lib;

using NetDiscoveryMobile.ViewModels;

namespace NetDiscoveryMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class InterfaceDetailPage : ContentPage
    {
        DiscoveryInterface viewModel;

        public InterfaceDetailPage(DiscoveryInterface viewModel)
        {
            BindingContext = this.viewModel = viewModel;

            InitializeComponent();
        }

        public InterfaceDetailPage()
        {
            InitializeComponent();

            //viewModel = new ItemDetailViewModel(item);
            //BindingContext = viewModel;
        }
    }
}