using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace TaskManagement.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetRequiredService<TasksViewModel>();
    }

}
