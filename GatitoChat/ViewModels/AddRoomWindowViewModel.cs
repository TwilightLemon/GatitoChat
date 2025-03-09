using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core.Models;
using GatitoChat.Services;

namespace GatitoChat.ViewModels;

public partial class AddRoomWindowViewModel(ChatClientService chatClientService):ViewModelBase
{
    [ObservableProperty]
    private string _roomName=string.Empty;
    public event Action? OnAddedRoom;
    [RelayCommand]
    private async Task CheckRoom()
    {
        await chatClientService.JoinRoom(RoomName);
        OnAddedRoom?.Invoke();
    }
}