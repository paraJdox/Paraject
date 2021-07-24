﻿using Paraject.Core.Commands;
using Paraject.Core.Enums;
using Paraject.Core.Repositories;
using Paraject.Core.Services.DialogService;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.MessageBoxes;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels
{
    public class TaskDetailsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly TaskRepository _taskRepository;
        private readonly Action _refreshTaskCollection;
        private readonly TasksViewModel _tasksViewModel;

        /// <summary>
        /// This displays the details of the selectedTask
        /// </summary>
        /// <param name="refreshTaskCollection">refreshes the Collection in the ChildView (TasksTodoView/CompletedTasksView) after a certain action is invoked</param>
        /// <param name="tasksViewModel">this is passed to save the UI state of TasksView when navigating back to it</param>
        /// <param name="selectedTask">the selected task in TasksTodoView/CompletedTasksview</param>
        public TaskDetailsViewModel(Action refreshTaskCollection, TasksViewModel tasksViewModel, Task selectedTask)
        {
            _dialogService = new DialogService();
            _taskRepository = new TaskRepository();

            _refreshTaskCollection = refreshTaskCollection;
            _tasksViewModel = tasksViewModel;
            CurrentTask = selectedTask;

            UpdateTaskCommand = new DelegateCommand(Update);
            DeleteTaskCommand = new DelegateCommand(Delete);
        }

        #region Properties
        public Task CurrentTask { get; set; }

        public ICommand UpdateTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        #endregion

        #region Methods
        private void Update()
        {
            if (TaskIsValid())
            {
                UpdateTaskAndShowResult(_taskRepository.Update(CurrentTask));
            }
        }
        private bool TaskIsValid()
        {
            if (TaskSubjectIsValid() == false)
            {
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Incorrect Data Entry", "A Task should have a subject.", Icon.InvalidTask));
                return false;
            }

            else if (TaskStatusCanBeCompleted() == false)
            {
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Update Operation", $"Unable to mark this Task's Status as \"Completed\" because there are still {CurrentTask.SubtaskCount} unfinished subtask/s remaining.", Icon.InvalidTask));
                return false;
            }

            return true;
        }
        private bool TaskSubjectIsValid()
        {
            return !string.IsNullOrWhiteSpace(CurrentTask.Subject);
        }
        private bool TaskStatusCanBeCompleted()
        {
            //A Task's status can only be marked "Completed" if they don't have any unfinished Subtasks
            if (CurrentTask.Status == "Completed")
            {
                return CurrentTask.SubtaskCount == 0;
            }

            return true;
        }
        private void UpdateTaskAndShowResult(bool isValid)
        {
            if (isValid)
            {
                _refreshTaskCollection();
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Update Operation", "Task Updated Successfully!", Icon.ValidTask));
                return;
            }

            _dialogService.OpenDialog(new OkayMessageBoxViewModel("Error", "An error occured, cannot update the Task.", Icon.InvalidTask));
        }

        private void Delete()
        {
            DialogResults result = _dialogService.OpenDialog(new YesNoMessageBoxViewModel("Delete Operation",
                                                             "Do you want to DELETE this task? \n\nAll Subtasks that belongs to this Task will also be deleted.",
                                                             Icon.Task));
            if (result == DialogResults.Yes)
            {
                DeleteProject();
            }
        }
        private void DeleteProject()
        {
            bool isDeleted = _taskRepository.Delete(CurrentTask.Id);
            if (isDeleted)
            {
                //redirect to TasksView(parent View) after a successful DELETE operation, and
                //refreshes the Tasks Collection in TasksTodoViewCompletedTasksView(child Views of TasksView) with the new records
                _refreshTaskCollection();
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Delete Operation", "Task Deleted Successfully!", Icon.ValidTask));
                MainWindowViewModel.CurrentView = _tasksViewModel;
            }
            else
            {
                _dialogService.OpenDialog(new OkayMessageBoxViewModel("Error", "An error occured, cannot delete the Task.", Icon.InvalidTask));
            }
        }
        #endregion
    }
}
