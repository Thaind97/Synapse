using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Synapse.Infrastructure.Entities;
using Synapse.Shared.Constants;

namespace Synapse.Presentation.Converters
{
    public class LoopCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DeviceCommand command)
            {
                if (!string.Equals(command.Command, SequenceConstants.CommandLoop, StringComparison.OrdinalIgnoreCase))
                    return string.Empty;

                var loopParam = command.CommandParameters?.FirstOrDefault(p => p.Name == SequenceConstants.LoopCountParameterName);
                return loopParam?.Value ?? string.Empty;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
