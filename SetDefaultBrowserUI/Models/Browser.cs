using System.Reflection;

namespace SetDefaultBrowserUI.Models;

public class Browser
{
    public Browser(string name, string hive, string identifier, string path)
    {
        Name = name; Hive = hive; Identifier = identifier; Path = path;
    }
    public string Name { get; }
    public string Hive { get; }
    public string Identifier { get; }
    public string Path { get; }
}