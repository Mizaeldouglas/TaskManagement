using TaskManagement.WPF.Models;

namespace TaskManagement.WPF.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(string title, string? description);
        Task DeleteTaskAsync(int id);
        Task<TaskItem> UpdateTaskStatusAsync(int id, string newStatus);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Models.TaskStatus status);
        Task<TaskItem> UpdateTaskAsync(int id, string newTaskTitle, string newTaskDescription, Models.TaskStatus status);
    }
}
