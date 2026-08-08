using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WorshipPad.Core.Enums;

namespace WorshipPad.Converters;

public class LogLevelToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is not LogLevel level)
            return Brushes.White;

        return level switch
        {
            LogLevel.Info =>
                Brushes.White,

            LogLevel.Warning =>
                Brushes.Gold,

            LogLevel.Error =>
                Brushes.Red,

            LogLevel.Critical =>
                Brushes.DarkRed,

            _ =>
                Brushes.White
        };
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}