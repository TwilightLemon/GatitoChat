using System;
using GatitoChat.Core.Models;

namespace GatitoChat.Services;

/// <summary>
/// user credential and server urls provider
/// </summary>
public class UserProfileService
{
    public UserCredential? Credential { get;private set; }
    public string AuthServerUrl { get; set; }= "https://auth.twlmgatito.cn/user/";
    public string ChatServerUri { get; set; }= "wss://gatitochatserver.azurewebsites.net/chat";
    public event Action? OnLoginCallback;
    public void Login(UserCredential credential, string authServerUrl, string chatServerUri)
    {
        Credential = credential;
        AuthServerUrl=authServerUrl;
        ChatServerUri=chatServerUri;
        OnLoginCallback?.Invoke();
    }
}