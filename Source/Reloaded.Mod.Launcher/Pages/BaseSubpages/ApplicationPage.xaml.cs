﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Reloaded.Mod.Launcher.Models.ViewModel;
using Reloaded.WPF.Pages.Animations;
using Reloaded.WPF.Pages.Animations.Enum;
using ApplicationSubPage = Reloaded.Mod.Launcher.Pages.BaseSubpages.ApplicationSubPages.Enum.ApplicationSubPage;

namespace Reloaded.Mod.Launcher.Pages.BaseSubpages
{
    /// <summary>
    /// Interaction logic for ApplicationPage.xaml
    /// </summary>
    public partial class ApplicationPage : ReloadedIIPage, IDisposable
    {
        public ApplicationViewModel ViewModel { get; set; }

        public ApplicationPage()
        {
            InitializeComponent();
            ViewModel = new ApplicationViewModel(IoC.Get<MainPageViewModel>().SelectedApplication, IoC.Get<ManageModsViewModel>());
            this.AnimateOutStarted += AnimateOutChild;
            this.AnimateOutStarted += Dispose;
        }

        ~ApplicationPage()
        {
            Dispose();
        }

        public void Dispose()
        {
            ViewModel?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void AnimateOutChild()
        {
            PageHost.CurrentPage.AnimateOut();
        }

        private void NonReloadedMod_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is FrameworkElement element)
                {
                    if (element.DataContext is Process process)
                    {
                        ViewModel.SelectedProcess = process;
                        ViewModel.Page = ApplicationSubPage.NonReloadedProcess;
                        ViewModel.RaisePagePropertyChanged();
                    }
                }
            }
        }

        private void Summary_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.Page = ApplicationSubPage.ApplicationSummary;
                ViewModel.RaisePagePropertyChanged();
            }
        }

        /* Animation/Title/Setup overrides */

        protected override Animation[] MakeExitAnimations()
        {
            return new Animation[]
            {
                new RenderTransformAnimation(-this.ActualWidth, RenderTransformDirection.Horizontal, RenderTransformTarget.Away, null, ResourceManipulator.Get<double>(XAML_EXITSLIDEANIMATIONDURATION)),
                new OpacityAnimation(ResourceManipulator.Get<double>(XAML_EXITFADEANIMATIONDURATION), 1, ResourceManipulator.Get<double>(XAML_EXITFADEOPACITYEND))
            };
        }

        protected override void OnAnimateInFinished()
        {
            if (!String.IsNullOrEmpty(this.Title))
                IoC.Get<WindowViewModel>().WindowTitle = $"{this.Title}: {ViewModel.ApplicationTuple.ApplicationConfig.AppName}";
        }

        private void LaunchApplication_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: Implement Launch Application

        }
    }
}