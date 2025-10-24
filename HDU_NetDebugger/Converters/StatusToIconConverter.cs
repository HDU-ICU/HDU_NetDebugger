using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using HDU_NetDebugger.Models;
using System;
using System.Diagnostics;
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
                CheckStatus.Passed => "checkmark_circle_filled",
                CheckStatus.Failed => "dismiss_circle_filled",
                CheckStatus.Checking => "arrow_sync_circle_filled",
                CheckStatus.UnChecked => "more_circle_filled",
                CheckStatus.Skipped => "arrow_circle_down_filled",
                _ => "info_filled"
            };
            return Application.Current?.FindResource(iconName) as StreamGeometry;
        }
        return null;

    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
