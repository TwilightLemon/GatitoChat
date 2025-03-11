using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core.Models;
using GatitoChat.Services;
using GatitoChat.Views;

namespace GatitoChat.ViewModels;

public partial class AddRoomWindowViewModel(ChatClientService chatClientService):ViewModelBase
{
    [ObservableProperty]private bool _chosenRemote = false;
    [RelayCommand]
    private void ChooseRemoteRoom()
    {
        ChosenRemote = true;
    }

    [RelayCommand]
    private void ChooseLocalRoom()
    {
        App.GetRequiredService<AddLocalServerWindow>().ShowDialog(App.MainWindow);
        RequestCloseWindow?.Invoke();
    }

    [ObservableProperty]
    private string _roomName=string.Empty;
    public event Action? RequestCloseWindow;
    [RelayCommand]
    private async Task CheckRoom()
    {
        if (string.IsNullOrWhiteSpace(RoomName)) return;
        await chatClientService.JoinRoom(RoomName);
        RequestCloseWindow?.Invoke();
    }
}