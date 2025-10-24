using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using HDU_NetDebugger.Models;
using System;
using System.Globalization;

namespace HDU_NetDebugger.Converters;

public class StatusToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CheckStatus status)
        {

            var iconName = status switch
            {
                CheckStatus.CheckedNoError => "CheckCircle",
                CheckStatus.CheckedWithError => "ErrorCircle",
                CheckStatus.Checking => "Sync",
                CheckStatus.UnChecked => "CloseCircle",
                _ => "Info"
            };
            return iconName;
        }
        return null;

    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
