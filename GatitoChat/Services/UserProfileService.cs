using System;
using GatitoChat.Core.Models;

namespace GatitoChat.Services;

public class UserProfileService
{
    public UserCredential? Credential { get;private set; }
    public string AuthServerUrl { get; set; }=string.Empty;
    public string ChatServerUri { get; set; }=string.Empty;
    public event Action? OnLoginCallback;
    public void Login(UserCredential credential, string authServerUrl, string chatServerUri)
    {
        Credential = credential;
        AuthServerUrl=authServerUrl;
        ChatServerUri=chatServerUri;
        OnLoginCallback?.Invoke();
    }
}