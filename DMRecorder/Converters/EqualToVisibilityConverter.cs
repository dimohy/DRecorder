using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMRecorder.Converters;

public class EqualToVisibilityConverter : IValueConverter
{
    public object? Value
    {
        get; set;
    }

    public Visibility EqualVisibility
    {
        get; set;
    }

    public Visibility NotEqualVisibility
    {
        get; set;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value?.Equals(Value) == true)
            return EqualVisibility;

        return NotEqualVisibility;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not Visibility v)
            return null!;

        if (Value is bool bValue)
        {
            return v == EqualVisibility ? bValue : !bValue;
        }
        else
            return null!;
    }
}
