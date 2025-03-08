using Avalonia.Controls;
using GatitoChat.ViewModels;

namespace GatitoChat.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow(MainWindowViewModel  viewModel)
    {
        DataContext = _viewModel = viewModel;
        InitializeComponent();
    }
}