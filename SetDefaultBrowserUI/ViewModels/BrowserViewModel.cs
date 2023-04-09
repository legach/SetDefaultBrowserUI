using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SetDefaultBrowserUI.Models;

namespace SetDefaultBrowserUI.ViewModels;

public class BrowserViewModel: ObservableRecipient
{
    private Browser _browserModel;
    private ICommand _copyPathCommand;
    private ICommand _copyIdentifierCommand;
    private string _iconPath;

    public Browser Model
    {
        get => _browserModel;
        set
        {
            SetProperty(ref _browserModel, value);
            _iconPath = GetIconPathByName(value.Name);
        }
    }

    public string IconPath => _iconPath;


    #region Commands

    public ICommand CopyPathCommand
    {
        get
        {
            return _copyPathCommand ??= new RelayCommand(() => { Clipboard.SetText(Model.Path); });
        }
    }

    public ICommand CopyIdentifierCommand
    {
        get
        {
            return _copyIdentifierCommand ??= new RelayCommand(() => { Clipboard.SetText(Model.Identifier); });
        }
    }

    #endregion

    private string GetIconPathByName(string name)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        var resourceName = "default_128x128.png";
        var lower = name.ToLower();

        if (lower.Contains("chrome"))
        {
            resourceName = "chrome_128x128.png";
        }
        else if (lower.Contains("edge"))
        {
            resourceName = "edge_128x128.png";
        }
        else if (lower.Contains("firefox"))
        {
            resourceName = "firefox_128x128.png";
        }
        else if (lower.Contains("opera"))
        {
            resourceName = "opera_128x128.png";
        }
        else if (lower.Contains("tor"))
        {
            resourceName = "tor_128x128.png";
        }
        else if (lower.Contains("yandex"))
        {
            resourceName = "yandex_128x128.png";
        }

        return $"/{assemblyName};component/Resources/BrowserIcons/{resourceName}";
    }
}