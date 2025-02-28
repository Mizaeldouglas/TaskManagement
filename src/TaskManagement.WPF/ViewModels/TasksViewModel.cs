using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManagement.WPF.Models;
using TaskManagement.WPF.Services;
using TaskManagement.WPF.ViewModels;

public class TasksViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private ObservableCollection<TaskItem> _tasks = new();
    private TaskItem? _selectedTask;
    private bool _isLoading;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private string _newTaskTitle = string.Empty;
    private string _newTaskDescription = string.Empty;
    private bool _isEditMode = false;
    private bool _isStatusEditMode = false;
    private string _newStatus = string.Empty;

    public TasksViewModel(ITaskService taskService)
    {
        _taskService = taskService;

        LoadTasksCommand = new AsyncRelayCommand(LoadTasksAsync);
        CreateTaskCommand = new AsyncRelayCommand(CreateTaskAsync, CanCreateTask);
        UpdateTaskCommand = new AsyncRelayCommand(UpdateTaskAsync, CanUpdateTask);
        DeleteTaskCommand = new AsyncRelayCommand(DeleteTaskAsync, () => SelectedTask != null);
        CancelEditCommand = new RelayCommand(CancelEdit);
        CancelStatsEditCommand = new RelayCommand(CancelStatusEdit);
        EditTaskCommand = new RelayCommand(EditTask, () => SelectedTask != null);
        UpdateTaskStatusCommand = new AsyncRelayCommand(UpdateTaskStatusAsync, CanUpdateTaskStatus);
        StartEditStatusCommand = new RelayCommand(StartEditStatus, () => SelectedTask != null);

        LoadTasksAsync().ConfigureAwait(false);
    }

    public ObservableCollection<TaskItem> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set
        {
            SetProperty(ref _selectedTask, value);
            (DeleteTaskCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            (EditTaskCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (UpdateTaskStatusCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            (StartEditStatusCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (SetProperty(ref _statusFilter, value))
            {
                LoadTasksAsync().ConfigureAwait(false);
            }
        }
    }
    public bool IsEditModeOrStatusEditMode
    {
        get => IsEditMode || IsStatusEditMode;
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set
        {
            SetProperty(ref _newTaskTitle, value);
            (CreateTaskCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
            (UpdateTaskCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public string NewTaskDescription
    {
        get => _newTaskDescription;
        set => SetProperty(ref _newTaskDescription, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
            {
                OnPropertyChanged(nameof(IsEditModeOrStatusEditMode));
            }
        }
    }

    public bool IsStatusEditMode
    {
        get => _isStatusEditMode;
        set
        {
            if (SetProperty(ref _isStatusEditMode, value))
            {
                OnPropertyChanged(nameof(IsEditModeOrStatusEditMode));
            }
        }
    }

    public string NewStatus
    {
        get => _newStatus;
        set
        {
            SetProperty(ref _newStatus, value);
            (UpdateTaskStatusCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    public ICommand LoadTasksCommand { get; }
    public ICommand CreateTaskCommand { get; }
    public ICommand UpdateTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand CancelStatsEditCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand UpdateTaskStatusCommand { get; }
    public ICommand StartEditStatusCommand { get; }

    public List<string> StatusOptions => new()
    {
        "All",
        TaskManagement.WPF.Models.TaskStatus.Pending.ToString(),
        TaskManagement.WPF.Models.TaskStatus.InProgress.ToString(),
        TaskManagement.WPF.Models.TaskStatus.Completed.ToString()
    };

    private async Task LoadTasksAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            IEnumerable<TaskItem> tasks;

            if (_statusFilter == "All")
            {
                tasks = await _taskService.GetAllTasksAsync();
            }
            else if (Enum.TryParse(_statusFilter, out TaskManagement.WPF.Models.TaskStatus status))
            {
                tasks = await _taskService.GetTasksByStatusAsync(status);
            }
            else
            {
                tasks = Array.Empty<TaskItem>();
            }

            Tasks = new ObservableCollection<TaskItem>(tasks);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading tasks: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateTaskAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var newTask = await _taskService.CreateTaskAsync(NewTaskTitle, NewTaskDescription);

            Tasks.Add(newTask);
            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanCreateTask()
    {
        return !string.IsNullOrWhiteSpace(NewTaskTitle) && !IsLoading;
    }

    private async Task UpdateTaskAsync()
    {
        if (SelectedTask == null) return;

        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var status = Enum.Parse<TaskManagement.WPF.Models.TaskStatus>(SelectedTask.Status);
            var updatedTask = await _taskService.UpdateTaskAsync(
                SelectedTask.Id,
                NewTaskTitle,
                NewTaskDescription,
                status);

            var index = Tasks.IndexOf(Tasks.First(t => t.Id == updatedTask.Id));
            Tasks[index] = updatedTask;

            IsEditMode = false;
            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            SelectedTask = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanUpdateTask()
    {
        return IsEditMode && !string.IsNullOrWhiteSpace(NewTaskTitle) && !IsLoading;
    }

    private async Task UpdateTaskStatusAsync()
    {
        if (SelectedTask == null) return;

        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            var updatedTask = await _taskService.UpdateTaskStatusAsync(
                SelectedTask.Id,
                NewStatus);

            var index = Tasks.IndexOf(Tasks.First(t => t.Id == updatedTask.Id));
            Tasks[index] = updatedTask;

            SelectedTask = updatedTask;
            IsStatusEditMode = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error updating task status: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }





    private bool CanUpdateTaskStatus()
    {
        return SelectedTask != null && !string.IsNullOrWhiteSpace(NewStatus) && !IsLoading;
    }

    private async Task DeleteTaskAsync()
    {
        if (SelectedTask == null) return;

        try
        {
            ErrorMessage = string.Empty;
            IsLoading = true;

            await _taskService.DeleteTaskAsync(SelectedTask.Id);

            Tasks.Remove(SelectedTask);
            SelectedTask = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelEdit()
    {
        IsEditMode = false;
        IsStatusEditMode = false;
        NewTaskTitle = string.Empty;
        NewTaskDescription = string.Empty;
        SelectedTask = null;
    }

    private void CancelStatusEdit()
    {
        IsEditMode = false;
        IsStatusEditMode = false;
        NewTaskTitle = string.Empty;
        NewTaskDescription = string.Empty;
        SelectedTask = null;
    }

    private void EditTask()
    {
        if (SelectedTask == null) return;

        IsEditMode = true;
        NewTaskTitle = SelectedTask.Title;
        NewTaskDescription = SelectedTask.Description ?? string.Empty;
    }

    private void StartEditStatus()
    {
        if (SelectedTask == null) return;

        IsStatusEditMode = true;
        NewStatus = SelectedTask.Status;
    }
}
