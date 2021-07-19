﻿using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.Core.Services.DialogService;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.MessageBoxes;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels.ModalDialogs
{
    public class AddProjectIdeaModalDialogViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly ProjectIdeaRepository _projectIdeaRepository;
        private readonly Action _refreshProjectIdeasCollection;

        public AddProjectIdeaModalDialogViewModel(Action refreshProjectIdeasCollection, int currentUserId)
        {
            _dialogService = new DialogService();
            _projectIdeaRepository = new ProjectIdeaRepository();
            CurrentProjectIdea = new ProjectIdea()
            {
                User_Id_Fk = currentUserId
            };

            _refreshProjectIdeasCollection = refreshProjectIdeasCollection;

            AddProjectIdeaCommand = new DelegateCommand(Add);
            CloseModalDialogCommand = new DelegateCommand(CloseModalDialog);
        }

        #region Properties
        public ProjectIdea CurrentProjectIdea { get; set; }
        public ICommand AddProjectIdeaCommand { get; }
        public ICommand CloseModalDialogCommand { get; }
        #endregion

        #region Methods
        private void Add()
        {
            if (!string.IsNullOrWhiteSpace(CurrentProjectIdea.Name))
            {
                bool isAdded = _projectIdeaRepository.Add(CurrentProjectIdea);
                AddOperationResult(isAdded);
            }

            else
            {
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Incorrect Data Entry", "A Subtask should have a subject.", "/UiDesign/Images/Logo/defaultProjectLogo.png"));
            }
        }
        private void AddOperationResult(bool isAdded)
        {
            if (isAdded)
            {
                _refreshProjectIdeasCollection();
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Add Operation", "Project Idea Created Successfully!", "/UiDesign/Images/Logo/defaultProjectLogo.png"));
                CloseModalDialog();
            }

            else
            {
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Error", "An error occured, cannot create the Project Idea.", "/UiDesign/Images/Logo/defaultProjectLogo.png"));
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
