using Makaretu.Dns;
using NetDiscovery.Lib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NetDiscoveryWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DiscoveryClient clt;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clt = this.DataContext as DiscoveryClient;
            //clt.Start();
            //clt.ParseResourceRecord(new PTRRecord() { Name=new DomainName("_services._dns-sd._udp.local"), DomainName=new DomainName("_ntp._udp.local") }, null);
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            clt.Start();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            clt.Stop();
        }

        private void Button_Refresh(object sender, RoutedEventArgs e)
        {
            clt.StartQueries();
        }

        private void Button_ClearLogs(object sender, RoutedEventArgs e)
        {
            clt.ClearLogs();
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            clt.ClearAll();
        }
    }
}
