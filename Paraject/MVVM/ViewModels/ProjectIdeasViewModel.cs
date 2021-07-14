﻿using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.ModalDialogs;
using Paraject.MVVM.ViewModels.Windows;
using Paraject.MVVM.Views.ModalDialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels
{
    public class ProjectIdeasViewModel
    {
        private readonly int _currentUserId;
        private readonly ProjectIdeaRepository _projectIdeaRepository;

        public ProjectIdeasViewModel(int currentUserId)
        {
            _projectIdeaRepository = new ProjectIdeaRepository();
            _currentUserId = currentUserId;

            ShowAddProjectIdeaModalDialogCommand = new DelegateCommand(ShowAddProjectIdeaModalDialog);

            DisplayProjectIdeas();
        }

        #region Properties
        public ObservableCollection<ProjectIdea> ProjectIdeas { get; set; }
        public ObservableCollection<GridTileData> ProjectIdeaCardsGrid { get; set; }
        public ICommand ShowAddProjectIdeaModalDialogCommand { get; }
        #endregion

        #region Methods
        private void DisplayProjectIdeas()
        {
            GetValuesForProjectIdeasCollection();
            SetNewGridDisplay();
            TaskCardGridLocation();
        }
        private void GetValuesForProjectIdeasCollection()
        {
            ProjectIdeas = null;
            ProjectIdeas = new ObservableCollection<ProjectIdea>(_projectIdeaRepository.GetAll(_currentUserId));
        }
        private void SetNewGridDisplay()
        {
            ProjectIdeaCardsGrid = null;
            ProjectIdeaCardsGrid = new ObservableCollection<GridTileData>();
        }
        private void TaskCardGridLocation()
        {
            int row = -1;
            int column = -1;

            //This is for a 3 column grid, with n number of rows
            for (int i = 0; i < ProjectIdeas.Count; i++)
            {
                if (column == 2)
                {
                    column = 0;
                }

                else
                {
                    column++;
                }

                if (i % 3 == 0)
                {
                    row++;
                }

                GridTileData td = new(ProjectIdeas[i], row, column);
                ProjectIdeaCardsGrid.Add(td);
            }
        }

        private void ShowAddProjectIdeaModalDialog()
        {
            MainWindowViewModel.Overlay = true;

            AddProjectIdeaModalDialogViewModel addProjectIdeaModalDialogViewModel = new AddProjectIdeaModalDialogViewModel(_currentUserId);

            AddProjectIdeaModalDialog addProjectIdeaModalDialog = new AddProjectIdeaModalDialog();
            addProjectIdeaModalDialog.DataContext = addProjectIdeaModalDialogViewModel;
            addProjectIdeaModalDialog.ShowDialog();
        }
        #endregion
    }
}
