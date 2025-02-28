using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskManagement.WPF.Services;

namespace TaskManagement.WPF;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services => _serviceProvider;

    private void ConfigureServices(IServiceCollection services)
    {
        // Configure HttpClient with base address
        services.AddHttpClient<ITaskService, TaskService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7001/"); // API base address
        });

        // Register ViewModels
        services.AddTransient<TasksViewModel>();

        // Register MainWindow
        services.AddTransient<MainWindow>();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}