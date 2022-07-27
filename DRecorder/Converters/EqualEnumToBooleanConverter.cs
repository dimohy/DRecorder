using DRecorder.Core;

using Microsoft.UI.Xaml.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRecorder.Converters;

public class RecordStateEqualToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null || parameter is null)
            return false;

        var parameterString = $"{parameter?.ToString()}";
        var @params = parameterString.Split('|', StringSplitOptions.TrimEntries);
        return @params.Any(x => value.Equals(Enum.Parse<RecordState>(x)) is true);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
            return null!;

        var param = Enum.Parse<RecordState>(parameter.ToString()!);
        return param;
    }
}
