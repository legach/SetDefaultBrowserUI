using System.Reflection;

namespace SetDefaultBrowserUI.Models;

public class Browser
{
    private string _iconPath = null;
    public Browser(string name, string hive, string identifier, string path)
    {
        Name = name; Hive = hive; Identifier = identifier; Path = path;
    }
    public string Name { get; }
    public string Hive { get; }
    public string Identifier { get; }
    public string Path { get; }
    public string IconPath
    {
        get { return _iconPath ??= GetIconPathByName(); }
    }

    private string GetIconPathByName()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        var resourceName = "default_128x128.png";
        var lower = Name.ToLower();

        if (lower.Contains("chrome"))
        {
            resourceName = "chrome_128x128.png";
        } else if (lower.Contains("edge"))
        {
            resourceName = "edge_128x128.png";
        } else if (lower.Contains("firefox"))
        {
            resourceName = "firefox_128x128.png";
        } else if (lower.Contains("opera"))
        {
            resourceName = "opera_128x128.png";
        } else if (lower.Contains("tor"))
        {
            resourceName = "tor_128x128.png";
        } else if (lower.Contains("yandex"))
        {
            resourceName = "yandex_128x128.png";
        }

        return $"/{assemblyName};component/Resources/BrowserIcons/{resourceName}";
    }
}