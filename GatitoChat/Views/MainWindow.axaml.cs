using Avalonia.Controls;
using GatitoChat.Services;
using GatitoChat.ViewModels;

namespace GatitoChat.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel  viewModel)
    {
        DataContext =  viewModel;
        InitializeComponent();
        Closing += MainWindow_Closing;
    }

    private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        await App.GetRequiredService<ChatClientService>().CloseAll();
    }
}