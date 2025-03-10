using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core.Models;
using GatitoChat.Models;
using GatitoChat.Services;
using GatitoChat.Views;

namespace GatitoChat.ViewModels;
/*
 * TODO: 封装ChatClient和AuthClient到Service, 这里只做响应
 */
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly UserProfileService _userProfileService;
    private readonly ChatClientService _chatClientService;

    public MainWindowViewModel(UserProfileService userProfileService, ChatClientService chatClientService)
    {
        _userProfileService = userProfileService;
        _chatClientService = chatClientService;
        RoomsInfo = chatClientService.Rooms;
        chatClientService.OnConnectionFailed += ChatClientService_OnConnectionFailed;
        chatClientService.OnConnectionSucceeded += ChatClientService_OnConnectionSucceeded;
        userProfileService.OnLoginCallback += UserProfileService_OnLoginCallback;
    }

    [ObservableProperty] private bool _isConnectionFailed=false;//usually assume connection succeed. 
    private void ChatClientService_OnConnectionFailed()
    {
        IsConnectionFailed = true;
    }
    
    private void ChatClientService_OnConnectionSucceeded()
    {
        IsConnectionFailed = false;
    }

    [RelayCommand]
    private async Task Reconnect()
    {
        await _chatClientService.ReConnect();
    }

    private void UserProfileService_OnLoginCallback()
    {
        LoginBtnHint = _userProfileService.Credential?.Username ?? "Login";
    }
    
    [ObservableProperty]private string _loginBtnHint= "Login";
    [RelayCommand]
    private void Login()
    {
        App.GetRequiredService<LoginWindow>().ShowDialog(App.MainWindow);
    }

    public ObservableCollection<RoomModel> RoomsInfo { get; }
    [ObservableProperty]private RoomModel? _selectedRoom;

    [RelayCommand]
    private void AddRoom()
    {
        App.GetRequiredService<AddRoomWindow>().ShowDialog(App.MainWindow);
    }

    [RelayCommand]
    private async Task LeaveRoom()
    {
        if (SelectedRoom == null) return;
        await _chatClientService.LeaveRoom(SelectedRoom);
        SelectedRoom = null;
    }
    
    [ObservableProperty]private string _message = "";

    [RelayCommand]
    private async Task SendMessage()
    {
        if(string.IsNullOrWhiteSpace(Message)||SelectedRoom==null)return;
        await _chatClientService.SendMessage(SelectedRoom,Message);
        Message = "";
    }
}