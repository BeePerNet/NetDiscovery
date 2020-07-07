using NetDiscovery.Lib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace NetDiscoveryMobile.Converters
{
    public class HierarchyExpanderWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DiscoveryZone subZone)
            {
                double cpt = 0;
                while (subZone.Parent != null)
                {
                    cpt++;
                    subZone = subZone.Parent;
                }
                return cpt * 20d;
            }
            return 20d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
