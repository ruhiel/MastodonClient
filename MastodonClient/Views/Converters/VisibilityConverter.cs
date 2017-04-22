using Mastonet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace MastodonClient.Views.Converters
{
    [ValueConversion(typeof(Mastonet.Visibility), typeof(string))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Mastonet.Visibility visibility)
            {
                switch (visibility)
                {
                    case Mastonet.Visibility.Direct:
                        return "ダイレクト";
                    case Mastonet.Visibility.Private:
                        return "非公開";
                    case Mastonet.Visibility.Public:
                        return "公開";
                    case Mastonet.Visibility.Unlisted:
                        return "未収載";
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                switch (str)
                {
                    case "ダイレクト":
                        return Mastonet.Visibility.Direct;
                    case "非公開":
                        return Mastonet.Visibility.Private;
                    case "公開":
                        return Mastonet.Visibility.Public;
                    case "未収載":
                        return Mastonet.Visibility.Unlisted;
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
