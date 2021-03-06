﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Reloaded.Mod.Launcher.Commands.Templates;
using Reloaded.Mod.Launcher.Models.ViewModel;

namespace Reloaded.Mod.Launcher.Commands.AddAppPage
{
    /// <summary>
    /// Command to be used by the <see cref="AddAppPage"/> which decides
    /// whether the current entry can be removed.
    /// </summary>
    public class DeleteApplicationCommand : WithCanExecuteChanged, ICommand, IDisposable
    {
        private readonly AddAppViewModel _addAppViewModel;

        public DeleteApplicationCommand(AddAppViewModel addAppViewModel)
        {
            _addAppViewModel = addAppViewModel;
            _addAppViewModel.MainPageViewModel.ApplicationsChanged += RaiseCanExecute;
            _addAppViewModel.PropertyChanged += AddAppViewModelOnPropertyChanged;
        }

        ~DeleteApplicationCommand()
        {
            Dispose();
        }

        public void Dispose()
        {
            _addAppViewModel.PropertyChanged -= AddAppViewModelOnPropertyChanged;
            _addAppViewModel.MainPageViewModel.ApplicationsChanged -= RaiseCanExecute;
            GC.SuppressFinalize(this);
        }


        /* Implementation */

        private void AddAppViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_addAppViewModel.Application))
                RaiseCanExecute(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /* ICommand. */

        public bool CanExecute(object parameter)
        {
            return _addAppViewModel.MainPageViewModel.Applications.Count > 0 && _addAppViewModel.Application != null;
        }

        public void Execute(object parameter)
        {
            // Find Application in Viewmodel's list and remove it.
            var app = _addAppViewModel.Application.Config;
            var entry = _addAppViewModel.MainPageViewModel.Applications.First(x => x.Config.Equals(app));
            _addAppViewModel.MainPageViewModel.Applications.Remove(entry);

            // Delete folder contents.
            var directory = Path.GetDirectoryName(entry.ConfigPath) ?? throw new InvalidOperationException(Errors.FailedToGetDirectoryOfApplication());
            Directory.Delete(directory, true);

            // File system watcher automatically updates collection in MainPageViewModel.Applications
        }
    }
}
