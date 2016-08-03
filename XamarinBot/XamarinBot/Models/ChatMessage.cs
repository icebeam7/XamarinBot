using System;
using System.Globalization;
using Xamarin.Forms;

namespace LabelTextColorSample
{
    public class TextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isIncoming = bool.Parse(value.ToString());
            return isIncoming ? Color.Green : Color.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isIncoming = bool.Parse(value.ToString());
            return !isIncoming ? Color.Silver : Color.Pink;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StringToOptionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isIncoming = bool.Parse(value.ToString());
            return !isIncoming ? LayoutOptions.StartAndExpand : LayoutOptions.EndAndExpand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

namespace XamarinBot.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsIncoming { get; set; }
    }
}
