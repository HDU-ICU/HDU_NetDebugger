using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Collections.ObjectModel;

namespace HDU_NetDebugger.Converters;

public class StringCollectionToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Collection<string> list)
        {
            return string.Join(Environment.NewLine, list);
        }
        return null;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
