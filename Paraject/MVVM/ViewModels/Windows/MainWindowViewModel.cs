﻿using Paraject.Core.Commands;
using Paraject.Core.Enums;
using Paraject.Core.Services.DialogService;
using Paraject.Core.Utilities;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.MessageBoxes;
using Paraject.MVVM.Views.Windows;
using System;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels.Windows
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private static bool _overlay;
        private static object _currentView;

        #region EventHandlers
        //Event handler for static property (MainWindowOverlay), since PropertyChanged.Fody (nuget package) doesn't notify static property changes
        //The static property name in this ViewModel is Overlay. The event name is therefore OverlayChanged (or else it will not notify the changes)
        public static event EventHandler OverlayChanged;
        public static event EventHandler CurrentViewChanged;
        #endregion

        #region Constructor
        public MainWindowViewModel(UserAccount currentUserAccount)
        {
            _dialogService = new DialogService();
            CurrentUserAccount = currentUserAccount;

            DashboardVM = new DashboardViewModel();
            ProjectsVM = new ProjectsViewModel(currentUserAccount.Id);
            UserAccountVM = new UserAccountViewModel(currentUserAccount);
            ProjectIdeasVM = new ProjectIdeasViewModel(currentUserAccount.Id);
            OptionsVM = new OptionsViewModel();

            CurrentView = ProjectsVM;

            DashboardViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = DashboardVM; });
            ProjectsViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = ProjectsVM; });
            UserAccountView = new ParameterizedDelegateCommand(o => { CurrentView = UserAccountVM; });
            ProjectIdeasViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = ProjectIdeasVM; });
            OptionsViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = OptionsVM; });

            LogoutCommand = new DelegateCommand(Logout);
        }
        #endregion

        #region Properties
        public UserAccount CurrentUserAccount { get; set; }

        public static bool Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                if (OverlayChanged is not null)
                    OverlayChanged(null, EventArgs.Empty);
            }
        }
        public static object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                if (CurrentViewChanged is not null)
                    CurrentViewChanged(null, EventArgs.Empty);
            }
        }

        public DashboardViewModel DashboardVM { get; set; }
        public ProjectsViewModel ProjectsVM { get; set; }
        public UserAccountViewModel UserAccountVM { get; set; }
        public ProjectIdeasViewModel ProjectIdeasVM { get; set; }
        public OptionsViewModel OptionsVM { get; set; }

        public ICommand DashboardViewCommand { get; }
        public ICommand ProjectsViewCommand { get; }
        public ICommand UserAccountView { get; }
        public ICommand ProjectIdeasViewCommand { get; }
        public ICommand OptionsViewCommand { get; }
        public ICommand LogoutCommand { get; }
        #endregion

        #region Methods
        public void Logout()
        {
            DialogResults result = _dialogService.OpenDialog(new YesNoMessageBoxViewModel("Confirm Logout", "Do you want Logout?", "/UiDesign/Images/Logo/defaultProjectLogo.png"));

            if (result == DialogResults.Yes)
            {
                ShowLoginWindow();
            }
        }
        public void CloseMainWindow()
        {
            if (CloseWindow.WinObject != null)
                CloseWindow.CloseParent();
        }
        private void ShowLoginWindow()
        {
            LoginWindow loginWindow = new();
            loginWindow.Show();
            CloseMainWindow();
        }
        #endregion
    }
}
