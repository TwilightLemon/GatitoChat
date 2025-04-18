using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;
using GatitoChat.Services;
using GatitoChat.Views;

namespace GatitoChat.ViewModels;

public partial class LoginWindowViewModel
    (IHttpClientFactory httpClientFactory,
        UserProfileService userProfileService)
    : ViewModelBase
{
    private CancellationTokenSource? cts = new();
    private const string OriGreeting = "Welcome to GatitoChat!";
    private const string OriHint = "Login via A Gatito Auth Server";
    [ObservableProperty] private string _greeting = OriGreeting;
    [ObservableProperty] private string _hint = OriHint;
    [ObservableProperty] private string _authGreeting = string.Empty;

    public string? Uid { get; set; }
    public string? VerificationCode { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Username { get; set; }

    [ObservableProperty]
    private string _authServerUrl  = userProfileService.AuthServerUrl;
    [ObservableProperty]
    private string _chatServerUri  =userProfileService.ChatServerUri;
    
    partial void OnAuthServerUrlChanged(string value)
    {
        userProfileService.AuthServerUrl = value;
    }
    partial void OnChatServerUriChanged(string value)
    {
        userProfileService.ChatServerUri = value;
    }

    [ObservableProperty]
    private bool _isCheckedUser;
    [ObservableProperty] private bool _isUserExisting;
    [ObservableProperty] private bool _authenticated;
    [ObservableProperty] private bool _isLoading;

    private AuthenticationClient? _authClient;
    private CheckResponse? _checkToken;
    private VerifierResponse? _verificationToken;

    private void Authenticate(UserCredential credential)
    {
        userProfileService.Login(credential, AuthServerUrl, ChatServerUri);
        AuthGreeting = $"Welcome🐱,  {credential.Username}!";
        Authenticated = true;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;
        if (string.IsNullOrEmpty(Password)) return;
        cts ??= new();
        
        IsLoading = true;
        if (await _authClient!.Login(Uid, Password,cts.Token) is { } res)
        {
            Authenticate(res);
        }
        else
        {
            MessageBox.Show("Wrong password!!! ");
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task CheckUser()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;
        cts ??= new();
        IsLoading = true;
        _authClient ??= new(httpClientFactory.CreateClient(App.PublicClientFlag), AuthServerUrl);
        var usr = await _authClient.CheckUser(Uid, cts.Token);
        if (usr is { Success: true })
        {
            Greeting = $"Welcome back, {usr.Username}!";
            IsUserExisting = true;
            IsCheckedUser = true;
        }
        else if(usr is { SessionId:not null })
        {
            //中断性变更：已存在的账号可以继续使用，不存在的需要先验证邮箱格式
            if (!TextUtils.EmailRegex().IsMatch(Uid))
            {
                MessageBox.Show("Invalid email address.");
                IsLoading = false;
                return;
            }

            IsUserExisting = false;
            IsCheckedUser = true;
            _checkToken = usr;
            var verifier = await _authClient!.VerifyEmail(usr, Uid, cts.Token);
            if (verifier is null || !verifier.Success)
            {
                MessageBox.Show("Failed to access auth server!!");
                IsLoading = false;
                return;
            }
            _verificationToken = verifier;
        }
        IsLoading = false;
    }

    [RelayCommand]
    private async Task Register()
    {
        if (string.IsNullOrWhiteSpace(Uid)) return;
        if (string.IsNullOrWhiteSpace(Password) || Password != ConfirmPassword) return;
        if (string.IsNullOrWhiteSpace(Username)) return;
        if (_checkToken is null||string.IsNullOrEmpty(VerificationCode)||_verificationToken is null) return;

        cts ??= new();
        IsLoading = true;

        var (success, res) = await _authClient!.Register(_checkToken, _verificationToken, VerificationCode,Username, Password, Uid,cts.Token);
        if (success && res != null)
        {
            //once register successfully, login
            await Login();
        }
        else
        {
            MessageBox.Show("Failed to register to GatitoAuth!!");
        }
            IsLoading = false;
    }

    [RelayCommand]
    private void BackToCheckUser()
    {
        CancelTask();
        Greeting = OriGreeting;
        IsCheckedUser = false;
        IsUserExisting = false;
    }

    public void CancelTask()
    {
        if (cts != null && cts.IsCancellationRequested)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            Debug.WriteLine("Login operation cancelled.");
        }
    }
}