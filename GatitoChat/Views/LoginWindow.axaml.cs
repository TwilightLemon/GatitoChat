﻿using Avalonia;
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
        Closing += LoginWindow_Closing;
    }

    private void LoginWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        _viewModel.CancelTask();
    }

    private async void UserProfileService_OnLogin()
    {
        _userProfileService.OnLoginCallback -= UserProfileService_OnLogin;
        await Task.Delay(1000);
        Close();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        EmailInputTb.Focus();
    }
}