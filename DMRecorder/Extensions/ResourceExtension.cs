namespace DMRecorder.Extensions;

using Microsoft.Windows.ApplicationModel.Resources;

public static class ResourceExtension
{
    private static readonly ResourceLoader _resLoader = new();

    public static string GetLocalized(this string resourceKey)
    {
        return _resLoader.GetString(resourceKey);
    }
}
