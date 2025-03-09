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
    }
}