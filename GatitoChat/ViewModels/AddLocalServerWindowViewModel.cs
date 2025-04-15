using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Services;
using GatitoChat.Views;

namespace GatitoChat.ViewModels;

public partial class AddLocalServerWindowViewModel(LocalChatService localChatService):ViewModelBase
{
    [ObservableProperty] private bool _asServer;
    [ObservableProperty] private string _ipAddress=string.Empty;
    [ObservableProperty] private int _port=8888;
    [ObservableProperty] private string _nickname=string.Empty;
    [ObservableProperty] private string _btnHint = "Create";

    public event Action? RequestCloseWindow;

    partial void OnAsServerChanged(bool value)
    {
        BtnHint=value?"Create":"Join";
    }

    [RelayCommand]
    private async Task CreateOrJoin()
    {
        bool success;
        if (AsServer)
        {
            success=await localChatService.LaunchServer(Port, Nickname);
        }
        else
        {
            success=await localChatService.JoinLocalRoom(IpAddress, Port, Nickname);
        }
        if(success)RequestCloseWindow?.Invoke();
        else
        {
            MessageBox.Show("Failed to create or join a local room.");
        }
    }
    
}