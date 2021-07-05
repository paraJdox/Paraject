﻿using Paraject.Core.Commands;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private static object _currentView;
        private readonly ProjectsViewModel _projectsViewModel;
        private readonly Action _refreshProjectsCollection;

        public TasksViewModel(ProjectsViewModel projectsViewModel, Action refreshProjectsCollection, Project currentProject)
        {
            _projectsViewModel = projectsViewModel;
            _refreshProjectsCollection = refreshProjectsCollection;
            CurrentProject = currentProject;

            //TasksView child Views (ViewModels)
            TasksTodoVM = new TasksTodoViewModel(this, currentProject.Id, "Finish_Line");
            CompletedTasksVM = new CompletedTasksViewModel(this, currentProject.Id);
            ProjectNotesVM = new ProjectNotesViewModel();
            ProjectDetailsVM = new ProjectDetailsViewModel(projectsViewModel, currentProject);

            CurrentView = TasksTodoVM;

            //TasksView child Views (Navigation)
            TasksTodoViewCommand = new ParameterizedDelegateCommand(NavigateToTasksTodoView);
            CompletedTasksViewCommand = new DelegateCommand(NavigateToCompletedTasksView);
            ProjectNotesViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = ProjectNotesVM; });
            ProjectDetailsViewCommand = new ParameterizedDelegateCommand(o => { CurrentView = ProjectDetailsVM; });
            NavigateBackToProjectsViewCommand = new DelegateCommand(NavigateBackToProjectsView);
        }

        public static event EventHandler CurrentViewChanged;

        #region Properties
        public Project CurrentProject { get; set; }
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

        //RadioButtons in TasksView
        public bool FinishLineButtonIsChecked { get; set; } = true; //default selected RadioButton
        public bool ExtraFeaturesButtonIsChecked { get; set; }
        public bool CompletedButtonIsChecked { get; set; }

        //TasksView child Views
        public TasksTodoViewModel TasksTodoVM { get; set; }
        public CompletedTasksViewModel CompletedTasksVM { get; set; }
        public ProjectNotesViewModel ProjectNotesVM { get; set; }
        public ProjectDetailsViewModel ProjectDetailsVM { get; set; }

        //Commands
        public ICommand NavigateBackToProjectsViewCommand { get; }
        public ICommand TasksTodoViewCommand { get; }
        public ICommand CompletedTasksViewCommand { get; }
        public ICommand ProjectNotesViewCommand { get; }
        public ICommand ProjectDetailsViewCommand { get; }
        #endregion

        #region Navigation Methods
        public void NavigateBackToProjectsView()
        {
            _refreshProjectsCollection();
            MainWindowViewModel.CurrentView = _projectsViewModel;
        }
        private void NavigateToTasksTodoView(object taskType) //the argument passed to this parameter is in TasksView (a "CommandParameter" from a Tab header)
        {
            TasksTodoVM = new TasksTodoViewModel(this, CurrentProject.Id, taskType.ToString());
            CurrentView = TasksTodoVM;
        }
        private void NavigateToCompletedTasksView()
        {
            CompletedTasksVM = new CompletedTasksViewModel(this, CurrentProject.Id);
            CurrentView = CompletedTasksVM;
        }
        #endregion
    }
}