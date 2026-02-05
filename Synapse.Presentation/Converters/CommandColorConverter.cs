using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Synapse.Shared.Constants;

namespace Synapse.Presentation.Converters
{
    public class CommandColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string deviceName)
            {
                if (deviceName == SequenceConstants.CommandWait) return new SolidColorBrush(Colors.Yellow);
                if (deviceName == SequenceConstants.CommandLoop) return new SolidColorBrush(Colors.LightGreen);
                if (deviceName == SequenceConstants.CommandDevice) return new SolidColorBrush(Colors.LightPink);
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
