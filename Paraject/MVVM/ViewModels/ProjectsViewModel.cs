﻿using Microsoft.Win32;
using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.Windows;
using Paraject.MVVM.Views.ModalDialogs;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectsViewModel(UserAccount userAccount)
        {
            //Repository
            _projectRepository = new ProjectRepository();

            //Models
            CurrentProject = new Project();
            CurrentUserAccount = userAccount;

            //Project Displays in ProjectsView
            AllProjectsCommand = new DelegateCommand(AllProjects);
            PersonalProjectsCommand = new DelegateCommand(PersonalProjects);
            PaidProjectsCommand = new DelegateCommand(PaidProjects);

            //Commands in the AddProjectsModalDialog
            AddProjectsDialogCommand = new DelegateCommand(ShowAddProjectsDialog);
            CloseModalDialogCommand = new DelegateCommand(SetProjectDefaultThenCloseModal);
            AddProjectCommand = new DelegateCommand(Add);
            AddProjectLogoCommand = new DelegateCommand(LoadProjectLogo);

            //Redirect to TasksView when a Project card is selected (to view a Project's tasks)
            TasksViewCommand = new ParameterizedDelegateCommand(NavigateToTasksView);

            //Default Project Display
            AllProjects();
        }

        #region Properties
        public ObservableCollection<Project> Projects { get; set; }
        public TasksViewModel TasksVM { get; set; }
        public ProjectsViewModel ProjectsVM { get; set; }

        #region Models
        public Project CurrentProject { get; set; }
        public UserAccount CurrentUserAccount { get; set; }
        #endregion

        #region Commands
        //Add Projects Commands
        public ICommand AddProjectCommand { get; }
        public ICommand AddProjectLogoCommand { get; }

        //Display Projects Commands
        public ICommand AllProjectsCommand { get; }
        public ICommand PersonalProjectsCommand { get; }
        public ICommand PaidProjectsCommand { get; }

        //Show and Close AddProjectModalDialog
        public ICommand AddProjectsDialogCommand { get; }
        public ICommand CloseModalDialogCommand { get; }

        //Redirect to TasksView
        public ICommand TasksViewCommand { get; }
        #endregion
        #endregion

        #region Methods
        public void NavigateToTasksView(object projectId) //the argument passed to this parameter is in ProjectsView (a "CommandParameter" for a Project card)
        {
            //the selected project card from ProjectsView
            Project selectedProject = _projectRepository.Get((int)projectId);

            //change CurrenView (of the MainWindow) to TasksView
            TasksVM = new TasksViewModel(this, selectedProject);
            MainWindowViewModel.CurrentView = TasksVM;
        }

        #region Add Project Methods
        public void Add()
        {
            //Validate if a Project has a name
            if (!string.IsNullOrWhiteSpace(CurrentProject.Name))
            {
                bool isAdded = _projectRepository.Add(CurrentProject, CurrentUserAccount.Id);
                AddValidatedProjectToDB(isAdded);
            }

            else
            {
                MessageBox.Show("A project should have a name");
            }
        }
        private void AddValidatedProjectToDB(bool isAdded)
        {
            if (isAdded)
            {
                MessageBox.Show("Project Created");

                //close the AddProjectModalDialog if  Creating a Project is successful
                //Set Project's default values after closing AddProjectModalDialog
                SetProjectDefaultThenCloseModal();
            }

            else
            {
                MessageBox.Show("Error occured, cannot create project");
            }
        }
        #endregion

        #region Display Project/s Methods
        public void AllProjects()
        {
            Projects = new ObservableCollection<Project>(_projectRepository.GetAll(CurrentUserAccount.Id));
        }
        public void PersonalProjects()
        {
            Projects = new ObservableCollection<Project>(_projectRepository.FindAll(CurrentUserAccount.Id, ProjectOptions.Personal));
        }
        public void PaidProjects()
        {
            Projects = new ObservableCollection<Project>(_projectRepository.FindAll(CurrentUserAccount.Id, ProjectOptions.Paid));
        }
        #endregion

        #region AddProjectModalDialog dialog
        public void ShowAddProjectsDialog()
        {
            //Show overlay from MainWindow
            MainWindowViewModel.Overlay = true;

            AddProjectModalDialog addProjectModalDialog = new AddProjectModalDialog();
            addProjectModalDialog.DataContext = this;
            addProjectModalDialog.ShowDialog();
        }
        private void LoadProjectLogo()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Select the project's logo",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png"
            };

            if (openFile.ShowDialog() == true)
            {
                CurrentProject.Logo = Image.FromFile(openFile.FileName);
            }
        }
        private void SetProjectDefaultThenCloseModal()
        {
            MainWindowViewModel.Overlay = false;

            //To erase the last input values in AddProjectModalDialog
            CurrentProject = null;
            CurrentProject = new Project();

            //Close the Modal
            foreach (Window currentModal in Application.Current.Windows)
            {
                if (currentModal.DataContext == this)
                {
                    currentModal.Close();
                }
            }
        }

        #endregion
        #endregion
    }
}
