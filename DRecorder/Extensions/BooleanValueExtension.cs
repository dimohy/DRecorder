using Microsoft.UI.Xaml.Markup;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRecorder.Extensions;

public sealed class BooleanValueExtension : MarkupExtension
{
    public bool Value
    {
        get; set;
    }

    public BooleanValueExtension()
    {
    }

    public BooleanValueExtension(bool value) => Value = value;


    protected override object ProvideValue() => Value!;
}
