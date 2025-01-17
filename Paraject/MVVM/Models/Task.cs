﻿using Paraject.Core.Enums;
using PropertyChanged;
using System;
using System.ComponentModel;

namespace Paraject.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]

    public class Task : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public int Id { get; set; }
        public int Project_Id_Fk { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Category { get; set; } = Enum.GetName(Categories.Backend);
        public string Priority { get; set; } = Enum.GetName(Priorities.Low);
        public DateTime? Deadline { get; set; }
        public DateTime DateCreated { get; set; }
        public int SubtaskCount { get; set; }
    }
}