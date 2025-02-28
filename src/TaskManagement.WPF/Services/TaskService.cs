using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TaskManagement.WPF.Models;

namespace TaskManagement.WPF.Services;

public class TaskService : ITaskService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        var response = await _httpClient.GetAsync("api/tasks");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<TaskItem>>(content, _jsonOptions) ?? Array.Empty<TaskItem>();
    }

    public async Task<TaskItem> GetTaskByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TaskItem>(content, _jsonOptions) ?? throw new Exception("Failed to deserialize task");
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Models.TaskStatus status)
    {
        var response = await _httpClient.GetAsync($"api/tasks/status/{status}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<TaskItem>>(content, _jsonOptions) ?? Array.Empty<TaskItem>();
    }

    public async Task<TaskItem> CreateTaskAsync(string title, string? description)
    {
        var task = new { Title = title, Description = description };
        var content = JsonSerializer.Serialize(task);
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/tasks", requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TaskItem>(responseContent, _jsonOptions) ??
            throw new Exception("Failed to deserialize created task");
    }

    public async Task<TaskItem> UpdateTaskAsync(int id, string title, string? description, Models.TaskStatus status)
    {
        var task = new
        {
            Id = id,
            Title = title,
            Description = description,
            Status = status
        };

        var content = JsonSerializer.Serialize(task);
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"api/tasks/{id}", requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TaskItem>(responseContent, _jsonOptions) ??
            throw new Exception("Failed to deserialize updated task");
    }

    public async Task<TaskItem> UpdateTaskStatusAsync(int id, string status)
    {
        var task = new { Id = id, Status = status };
        var content = JsonSerializer.Serialize(task);
        var requestContent = new StringContent(content, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"api/tasks/{id}/status", requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonSerializer.Deserialize<TaskItem>(responseContent, _jsonOptions) ??
                throw new Exception("Failed to deserialize updated task");
        }
        catch (JsonException)
        {
            return await GetTaskByIdAsync(id);
        }
    }


    public async Task DeleteTaskAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
}