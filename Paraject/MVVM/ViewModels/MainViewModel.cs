﻿using Paraject.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paraject.MVVM.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        public NavigationCommand DashboardViewCommand { get; set; }
        public NavigationCommand ProjectsViewCommand { get; set; }
        public NavigationCommand ProfileViewCommand { get; set; }
        public NavigationCommand ProjectIdeasViewCommand { get; set; }
        public NavigationCommand OptionsViewCommand { get; set; }

        public DashboardViewModel DashboardVM { get; set; }
        public ProjectsViewModel ProjectsVM { get; set; }
        public ProfileViewModel ProfileVM { get; set; }
        public ProjectIdeasViewModel ProjectIdeasVM { get; set; }
        public OptionsViewModel OptionsVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            DashboardVM = new DashboardViewModel();
            ProjectsVM = new ProjectsViewModel();
            ProfileVM = new ProfileViewModel();
            ProjectIdeasVM = new ProjectIdeasViewModel();
            OptionsVM = new OptionsViewModel();

            CurrentView = DashboardVM;

            DashboardViewCommand = new NavigationCommand(o =>
            {
                CurrentView = DashboardVM;
            });

            ProjectsViewCommand = new NavigationCommand(o =>
            {
                CurrentView = ProjectsVM;
            });

            ProfileViewCommand = new NavigationCommand(o =>
            {
                CurrentView = ProfileVM;
            });

            ProjectIdeasViewCommand = new NavigationCommand(o =>
            {
                CurrentView = ProjectIdeasVM;
            });

            OptionsViewCommand = new NavigationCommand(o =>
            {
                CurrentView = OptionsVM;
            });
        }
    }
}
