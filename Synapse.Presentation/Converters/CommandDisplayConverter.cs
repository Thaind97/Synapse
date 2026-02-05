using System;
using System.Globalization;
using System.Windows.Data;
using Synapse.Infrastructure.Entities;

namespace Synapse.Presentation.Converters
{
    public class CommandDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DeviceCommand command)
            {
                return string.Empty;
            }

            var prefix = command.ParentCommandId != null ? "? (Loop) " : string.Empty;
            return $"{prefix}{command.Command}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
