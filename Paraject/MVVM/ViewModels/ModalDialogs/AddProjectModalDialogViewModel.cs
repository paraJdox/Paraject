﻿using Microsoft.Win32;
using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels.ModalDialogs
{
    public class AddProjectModalDialogViewModel : BaseViewModel
    {
        private readonly ProjectRepository _projectRepository;
        private readonly int _currentUserId;

        public AddProjectModalDialogViewModel(int currentUserId)
        {
            _projectRepository = new ProjectRepository();
            _currentUserId = currentUserId;

            CurrentProject = new Project();

            AddProjectCommand = new DelegateCommand(Add);
            AddProjectLogoCommand = new DelegateCommand(LoadProjectLogo);
            CloseModalDialogCommand = new DelegateCommand(CloseModalDialog);
        }

        #region Properties
        public Project CurrentProject { get; set; }
        public ICommand AddProjectCommand { get; }
        public ICommand AddProjectLogoCommand { get; }
        public ICommand CloseModalDialogCommand { get; }
        #endregion

        #region Methods
        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(CurrentProject.Name))
            {
                bool isAdded = _projectRepository.Add(CurrentProject, _currentUserId);
                AddOperationResult(isAdded);
            }

            else
            {
                MessageBox.Show("A project should have a name");
            }
        }
        private void AddOperationResult(bool isAdded)
        {
            if (isAdded)
            {
                ProjectsViewModel projectsViewModel = new(_currentUserId);
                MainWindowViewModel.CurrentView = projectsViewModel;

                MessageBox.Show("Project Created");
                CloseModalDialog();
            }

            else
            {
                MessageBox.Show("Error occured, cannot create project");
            }
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
                try
                {
                    CurrentProject.Logo = Image.FromFile(openFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Please input a valid image. \n{ex.ToString()}");
                }
            }
        }
        private void CloseModalDialog()
        {
            MainWindowViewModel.Overlay = false;

            SetProjectDefaultValues();

            foreach (Window currentModal in Application.Current.Windows)
            {
                if (currentModal.DataContext == this)
                {
                    currentModal.Close();
                }
            }
        }
        private void SetProjectDefaultValues()
        {
            //To erase the last input values in AddProjectModalDialog
            CurrentProject = null;
            CurrentProject = new Project();
        }
        #endregion
    }
}