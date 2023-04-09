using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SetDefaultBrowserUI.Models;
using SetDefaultBrowserUI.Services;
using SetDefaultBrowserUI.Utils;

namespace SetDefaultBrowserUI.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        private readonly SdbWrapper _wrapper;
        private BrowserViewModel? _selectedBrowser;
        private bool _isloaderVisible = false;
        private ObservableCollection<BrowserViewModel> _browsers;
        private ICommand _setBrowserCommand;
        private ICommand _refreshListCommand;
        private ICommand _loadedCommand;
        private ICommand _closingCommand;
        private ICommand _notifyIconOpenCommand;
        private ICommand _notifyIconExitCommand;
        private SynchronizationContext _syncContext = SynchronizationContext.Current;
        private bool _showInTaskbar;
        private WindowState _windowState ;
        private NotifyIconWrapper.NotifyRequestRecord? _notifyRequest;

        public MainViewModel(SdbWrapper wrapper)
        {
            _wrapper = wrapper;
            Browsers = new ObservableCollection<BrowserViewModel>();
            RunWithWaiting(FillAvailableBrowsers);
        }

        #region Properties
  
        public BrowserViewModel? SelectedBrowser
        {
            get => _selectedBrowser;
            set
            {
                SetProperty(ref _selectedBrowser, value);
                ((RelayCommand)SetBrowserCommand).NotifyCanExecuteChanged();
            }
        }

        public bool IsLoaderVisible
        {
            get=>_isloaderVisible;
            set => SetProperty(ref _isloaderVisible, value);
        }

        public ObservableCollection<BrowserViewModel> Browsers
        {
            get => _browsers;
            set
            {
                if (_browsers == value)
                    return;
                SetProperty(ref _browsers, value);
            }
        }

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                ShowInTaskbar = true;
                SetProperty(ref _windowState, value);
                ShowInTaskbar = value != WindowState.Minimized;
            }
        }

        public bool ShowInTaskbar
        {
            get => _showInTaskbar;
            set => SetProperty(ref _showInTaskbar, value);
        }

        public NotifyIconWrapper.NotifyRequestRecord? NotifyRequest
        {
            get => _notifyRequest;
            set => SetProperty(ref _notifyRequest, value);
        }

        #endregion

        #region Commands
        public ICommand SetBrowserCommand
        {
            get
            {
                return _setBrowserCommand ??= new RelayCommand(
                    async () => await RunWithWaiting(SetBrowsersAsDefault),
                    () =>
                    {
                        return SelectedBrowser != null;
                    });
            }
        }

        public ICommand RefreshListCommand
        {
            get
            {
                return _refreshListCommand ??= new RelayCommand(
                    async () => await RunWithWaiting(FillAvailableBrowsers)
                );
            }
        }

        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??= new RelayCommand(Loaded);
            }
        }

        public ICommand ClosingCommand
        {
            get
            {
                return _closingCommand ??= new RelayCommand<CancelEventArgs>(Closing);
            }
        }

        public ICommand NotifyIconOpenCommand
        {
            get
            {
                return _notifyIconOpenCommand ??= new RelayCommand(() =>
                {
                    WindowState = WindowState.Normal;
                });
            }
        }

        public ICommand NotifyIconExitCommand
        {
            get
            {
                return _notifyIconExitCommand ??= new RelayCommand(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
        #endregion

        private void Notify(string message)
        {
            NotifyRequest = new NotifyIconWrapper.NotifyRequestRecord
            {
                Title = "Notify",
                Text = message,
                Duration = 1000
            };
        }
        private void Loaded()
        {
            WindowState = WindowState.Normal;
        }

        private void Closing(CancelEventArgs? e)
        {
            if (e == null)
                return;
            e.Cancel = true;
            WindowState = WindowState.Minimized;
        }

        private async void SetBrowsersAsDefault()
        {
            var result = await _wrapper.SetBrowser(SelectedBrowser.Model);
            if (!result.IsSuccess)
            {
                ShowError(result.Error);
                return;
            }
        }

        private async void FillAvailableBrowsers()
        {
            var result = await _wrapper.GetAvailableBrowsers();
            if (!result.IsSuccess)
            {
                ShowError(result.Error);
                return;
            }

            var browserViewModels = result.Result.Select(b=>new BrowserViewModel(){Model = b}).ToList();
            Browsers = new ObservableCollection<BrowserViewModel>(browserViewModels);
        }

        private void ShowError(string resultError)
        {
            MessageBox.Show(resultError, "Something went wrong");
        }

        private async Task RunWithWaiting(Action action)
        {
            IsLoaderVisible = true;
            await Task.Run(() => { action(); });
            IsLoaderVisible = false;
        }
    }
}
