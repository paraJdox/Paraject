﻿using Microsoft.Win32;
using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.Core.Services.DialogService;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.MessageBoxes;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels.ModalDialogs
{
    public class AddProjectModalDialogViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ProjectRepository _projectRepository;
        private readonly Action _refreshProjectsCollection;

        public AddProjectModalDialogViewModel(Action refreshProjectsCollection, int currentUserId)
        {
            _dialogService = new DialogService();
            _projectRepository = new ProjectRepository();
            _refreshProjectsCollection = refreshProjectsCollection;

            CurrentProject = new Project()
            {
                User_Id_Fk = currentUserId
            };

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
                bool isAdded = _projectRepository.Add(CurrentProject);
                AddOperationResult(isAdded);
            }

            else
            {
                string iconSource = "/UiDesign/Images/Logo/defaultProjectLogo.png";

                OkayMessageBoxViewModel okayMessageBox = new("Incorrect Data Entry", "A Project should have a name.", iconSource);
                _dialogService.OpenDialog(okayMessageBox);
            }
        }
        private void AddOperationResult(bool isAdded)
        {
            string iconSource = "/UiDesign/Images/Logo/defaultProjectLogo.png";

            if (isAdded)
            {
                _refreshProjectsCollection();

                OkayMessageBoxViewModel okayMessageBox = new("Add Operation", "Project Created Successfully!", iconSource);
                _dialogService.OpenDialog(okayMessageBox);

                CloseModalDialog();
            }

            else
            {
                OkayMessageBoxViewModel okayMessageBox = new("Error", "An error occured, cannot create the Project.", iconSource);
                _dialogService.OpenDialog(okayMessageBox);
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
                    string iconSource = "/UiDesign/Images/Logo/defaultProjectLogo.png";

                    OkayMessageBoxViewModel okayMessageBox = new("Image format Error", $"Please select a valid image.\n \n{ex}", iconSource);
                    _dialogService.OpenDialog(okayMessageBox);
                }
            }
        }
        private void CloseModalDialog()
        {
            MainWindowViewModel.Overlay = false;

            foreach (Window currentModal in Application.Current.Windows)
            {
                if (currentModal.DataContext == this)
                {
                    currentModal.Close();
                }
            }
        }
        #endregion
    }
}
