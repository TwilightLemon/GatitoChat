using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GatitoChat.Services;
using GatitoChat.ViewModels;
using System;
using System.Threading.Tasks;

namespace GatitoChat.Views;

public partial class LoginWindow : Window
{
    private readonly UserProfileService _userProfileService;
    private readonly LoginWindowViewModel _viewModel;

    #if DEBUG
    public LoginWindow()
    {
        InitializeComponent();
    }
    #endif
    public LoginWindow(LoginWindowViewModel viewModel,UserProfileService userProfileService)
    {
        InitializeComponent();
        _userProfileService = userProfileService;
        DataContext = _viewModel = viewModel;
        userProfileService.OnLoginCallback += UserProfileService_OnLogin;
    }

    private async void UserProfileService_OnLogin()
    {
        _userProfileService.OnLoginCallback -= UserProfileService_OnLogin;
        await Task.Delay(1500);
        Close();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        EmailInputTb.Focus();
    }
}