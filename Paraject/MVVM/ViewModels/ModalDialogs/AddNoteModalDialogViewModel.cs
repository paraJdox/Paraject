﻿using Paraject.Core.Commands;
using Paraject.Core.Repositories;
using Paraject.MVVM.Models;
using Paraject.MVVM.ViewModels.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace Paraject.MVVM.ViewModels.ModalDialogs
{
    public class AddNoteModalDialogViewModel : BaseViewModel
    {
        private readonly NoteRepository _noteRepository;
        private readonly Action _refreshNotesCollection;

        public AddNoteModalDialogViewModel(Action refreshNotesCollection, int currentProjectId)
        {
            _noteRepository = new NoteRepository();
            _refreshNotesCollection = refreshNotesCollection;

            CurrentNote = new Note()
            {
                Project_Id_Fk = currentProjectId
            };

            CloseModalDialogCommand = new DelegateCommand(CloseModalDialog);
            AddNoteCommand = new DelegateCommand(Add);
        }

        #region Properties
        public Note CurrentNote { get; set; }

        public ICommand AddNoteCommand { get; }
        public ICommand CloseModalDialogCommand { get; }
        #endregion

        #region Methods
        public void Add()
        {
            if (!string.IsNullOrWhiteSpace(CurrentNote.Subject))
            {
                bool isAdded = _noteRepository.Add(CurrentNote);
                AddOperationResult(isAdded);
            }

            else
            {
                MessageBox.Show("A note should have a subject");
            }
        }
        private void AddOperationResult(bool isAdded)
        {
            if (isAdded)
            {
                //refresh Notes collection in NotesView//
                _refreshNotesCollection();
                MessageBox.Show("Note Created");
                CloseModalDialog();
            }

            else
            {
                MessageBox.Show("Error occured, cannot create note");
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
