using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using HDU_NetDebugger.Models;
using System;
using System.Globalization;

namespace HDU_NetDebugger.Converters;

public class StatusToIconColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CheckStatus status)
        {
            var colorName = status switch
            {
                CheckStatus.Passed => "Green",
                CheckStatus.Warned => "Yellow",
                CheckStatus.Failed => "Red",
                CheckStatus.Checking => "Blue",
                CheckStatus.UnChecked => "Gray",
                CheckStatus.Skipped => "Orange",
                _ => "Black"
            };
            var color = SolidColorBrush.Parse(colorName);
            Console.WriteLine($"Converted {status} to color {colorName}, found brush: {color}");
            return color;
        }
        return null;

    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
