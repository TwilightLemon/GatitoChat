using Avalonia.Controls;
using GatitoChat.ViewModels;

namespace GatitoChat.Views;

public partial class AddLocalServerWindow : Window
{
    private readonly AddLocalServerWindowViewModel _viewModel;
    #if DEBUG
    public AddLocalServerWindow()
    {
        InitializeComponent();
    }
    #endif

    public AddLocalServerWindow(AddLocalServerWindowViewModel viewModel)
    {
        DataContext = _viewModel = viewModel;
        viewModel.RequestCloseWindow += ViewModel_RequestCloseWindow;
        InitializeComponent();
    }

    private void ViewModel_RequestCloseWindow()
    {
        _viewModel.RequestCloseWindow -= ViewModel_RequestCloseWindow;
        Close();
    }
}