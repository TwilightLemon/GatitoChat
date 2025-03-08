using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core.Models;
using GatitoChat.Services;

namespace GatitoChat.ViewModels;

public partial class AddRoomWindowViewModel(CommonClientService commonClientService):ViewModelBase
{
    [ObservableProperty]
    private string _roomName=string.Empty;
    [ObservableProperty]
    private string _hint=string.Empty;
    public event Action<RoomInfo>? AddRoomSuccess;
    [RelayCommand]
    private async Task CheckRoom()
    {
        if (string.IsNullOrWhiteSpace(RoomName)) return;
        AddRoom(RoomName,"111");
        return;
        if (commonClientService.RoomManager is null) return;

        var res=await commonClientService.RoomManager.CheckRoom(RoomName);
        if (res is { Success: true }msg)
        {
            AddRoom(RoomName,msg.Message);
        }
        else
        {
            Hint = "No room found, now creating a new one for you";
            var created=await commonClientService.RoomManager.CreateRoom(RoomName);
            if(created is {Success: true} newRoom)
            {
                AddRoom(RoomName, newRoom.Message);
            }
        }
    }

    private void AddRoom(string roomName, string roomHash)
    {
        Hint=$"Added room {roomName} successfully";
        AddRoomSuccess?.Invoke(new(RoomName,roomHash));
    }
}