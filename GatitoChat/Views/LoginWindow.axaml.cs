using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GatitoChat.ViewModels;

namespace GatitoChat.Views;

public partial class LoginWindow : Window
{
    private readonly LoginWindowViewModel _viewModel;

    public LoginWindow(LoginWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = _viewModel = viewModel;
        viewModel.OnLoginCallback += ViewModel_OnLoginCallBack;
    }

    private async void ViewModel_OnLoginCallBack()
    {
        _viewModel.OnLoginCallback -= ViewModel_OnLoginCallBack;
        await Task.Delay(1500);
        Close();
    }
}