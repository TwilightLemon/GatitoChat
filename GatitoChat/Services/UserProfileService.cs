using GatitoChat.Core.Models;

namespace GatitoChat.Services;

public class UserProfileService
{
    public LoginResponse Credential { get; set; } = new();
    public string AuthServerUrl { get; set; }=string.Empty;
    public string ChatServerUrl { get; set; }=string.Empty;
}