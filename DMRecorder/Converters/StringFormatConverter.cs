using System;

using Microsoft.UI.Xaml.Data;

namespace DMRecorder.Converters;

public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is null)
            return null!;

        if (parameter is not string stringParameter)
            return value;

        return string.Format(stringParameter, value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
