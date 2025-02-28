namespace TaskManagement.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty", nameof(Title));

            if (value.Length > 100)
                throw new ArgumentException("Title cannot exceed 100 characters", nameof(Title));

            _title = value;
        }
    }

    public string? Description { get; set; }

    public DateTime CreationDate { get; private set; }

    private DateTime? _completionDate;
    public DateTime? CompletionDate
    {
        get => _completionDate;
        set
        {
            if (value.HasValue && value.Value < CreationDate)
                throw new ArgumentException("Completion date cannot be before creation date", nameof(CompletionDate));

            _completionDate = value;

            if (value.HasValue)
                Status = TaskStatus.Completed;
        }
    }

    public TaskStatus Status { get; set; }

    public TaskItem(string title, string? description = null)
    {
        Title = title;
        Description = description;
        CreationDate = DateTime.UtcNow;
        Status = TaskStatus.Pending;
    }


    private TaskItem()
    {
        CreationDate = DateTime.UtcNow;
    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    public void MarkAsInProgress()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Cannot mark a completed task as in progress");

        Status = TaskStatus.InProgress;
    }

    public void MarkAsCompleted()
    {
        Status = TaskStatus.Completed;
        CompletionDate = DateTime.UtcNow;
    }

    public void MarkAsPending()
    {
        if (CompletionDate.HasValue)
            CompletionDate = null;

        Status = TaskStatus.Pending;
    }
}
