using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Collections.ObjectModel;

namespace HDU_NetDebugger.Converters;

public class StringCollectionToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Collection<string> list)
        {
            return list.Count > 0;
        }
        return false;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
