using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core;
using GatitoChat.Core.Models;
using GatitoChat.Services;

namespace GatitoChat.ViewModels;

public partial class LoginWindowViewModel(IHttpClientFactory httpClientFactory, UserProfileService userProfileService)
    : ViewModelBase
{
    private const string OriGreeting = "Welcome to GatitoChat!";
    private const string OriHint = "Login via A Gatito Auth Server";
    [ObservableProperty] private string _greeting = OriGreeting;
    [ObservableProperty] private string _hint = OriHint;
    [ObservableProperty] private string _authGreeting = string.Empty;

    public string? Uid { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }

    public string? Username { get; set; }

    public string AuthServerUrl { get; set; } = "https://gatito-auth.vercel.app/user/";
    public string ChatServerUrl { get; set; } = "http://127.0.0.1:8080/";

    [ObservableProperty] private bool _isCheckedUser;
    [ObservableProperty] private bool _isUserExisting;
    [ObservableProperty] private bool _authenticated;

    public event Action? OnLoginCallback;

    private AuthenticationClient? _authClient;

    private void Authenticate(LoginResponse credential)
    {
        userProfileService.Credential = credential;
        userProfileService.AuthServerUrl=AuthServerUrl;
        userProfileService.ChatServerUrl=ChatServerUrl;
        AuthGreeting = $"Welcome🐱,  {credential.UserName}!";
        Authenticated = true;
        OnLoginCallback?.Invoke();
    }

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;
        if (string.IsNullOrEmpty(Password)) return;
        
        if (await _authClient!.Login(Uid, Password) is
            { Success: true} res)
        {
            Authenticate(res);
        }
        else
        {
            Hint="Wrong password";
            await Task.Delay(3000);
            Hint = OriHint;
        }
    }

    [RelayCommand]
    private async Task CheckUser()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;

        _authClient ??= new(httpClientFactory.CreateClient(App.PublicClientFlag), AuthServerUrl);
        if (await _authClient.CheckUser(Uid) is { Success: true } usr)
        {
            Greeting = $"Welcome back, {usr.UserName}!";

            IsUserExisting = true;
            IsCheckedUser = true;
        }
        else
        {
            IsUserExisting = false;
            IsCheckedUser = true;
        }
    }

    [RelayCommand]
    private async Task Register()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;
        if (string.IsNullOrWhiteSpace(Password) || Password != ConfirmPassword) return;
        if (string.IsNullOrWhiteSpace(Username)) return;

        var (success, res) = await _authClient!.Register(Username, Password, Uid);
        if (success && res != null)
        {
            //once register successfully, login
            await Login();
        }
    }

    [RelayCommand]
    private void BackToCheckUser()
    {
        Greeting = OriGreeting;
        IsCheckedUser = false;
        IsUserExisting = false;
    }
}