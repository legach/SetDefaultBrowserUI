using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SetDefaultBrowserUI.Models;
using SetDefaultBrowserUI.Services;
using SetDefaultBrowserUI.Utils;

namespace SetDefaultBrowserUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SdbWrapper _wrapper;
        private Browser? _selectedBrowser;
        private bool _isloaderVisible = false;
        private ObservableCollection<Browser> _browsers;
        private RelayCommand _setBrowserCommand;
        private RelayCommand _refreshListCommand;
        private SynchronizationContext _syncContext = SynchronizationContext.Current;

        public MainViewModel(SdbWrapper wrapper)
        {
            _wrapper = wrapper;
            Browsers = new ObservableCollection<Browser>();
            RunWithWaiting(FillAvailableBrowsers);
        }

        public Browser? SelectedBrowser
        {
            get => _selectedBrowser;
            set { _selectedBrowser = value; OnPropertyChanged(nameof(SelectedBrowser));}
        }

        public bool IsLoaderVisible
        {
            get=>_isloaderVisible;
            set { _isloaderVisible = value; OnPropertyChanged(nameof(IsLoaderVisible)); }
        }

        public ObservableCollection<Browser> Browsers
        {
            get => _browsers;
            set
            {
                if (_browsers == value)
                    return;
                _browsers = value;
                OnPropertyChanged(nameof(Browser));
            }
        }

        public RelayCommand SetBrowserCommand
        {
            get
            {
                return _setBrowserCommand ??= new RelayCommand(
                    async o => await RunWithWaiting(SetBrowsersAsDefault),
                    o => SelectedBrowser != null);
            }
        }

        public RelayCommand RefreshListCommand
        {
            get
            {
                return _refreshListCommand ??= new RelayCommand(
                     o => RunWithWaiting(FillAvailableBrowsers)
                );
            }
        }

        private async void SetBrowsersAsDefault()
        {
            var result = await _wrapper.SetBrowser(SelectedBrowser);
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

            Browsers = new ObservableCollection<Browser>(result.Result);
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


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
