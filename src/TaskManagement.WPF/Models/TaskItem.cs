// src/TaskManagement.WPF/Models/TaskItem.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManagement.WPF.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }

        private DateTime? _completionDate;
        public DateTime? CompletionDate
        {
            get => _completionDate;
            set
            {
                if (_completionDate != value)
                {
                    _completionDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CompletionDateFormatted));
                }
            }
        }

        public string Status { get; set; } = string.Empty;

        public string CreationDateFormatted => CreationDate.ToString("dd/MM/yyyy HH:mm");
        public string CompletionDateFormatted => CompletionDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
