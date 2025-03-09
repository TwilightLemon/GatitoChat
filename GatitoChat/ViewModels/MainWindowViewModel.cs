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
        userProfileService.OnLoginCallback += UserProfileService_OnLoginCallback;
    }

    private void UserProfileService_OnLoginCallback()
    {
        LoginBtnHint = _userProfileService.Credential?.Username ?? "Login";
    }
    
    [ObservableProperty]private string _loginBtnHint= "Login";
    [RelayCommand]
    private void Login()
    {
        App.GetRequiredService<LoginWindow>().Show();
    }

    public ObservableCollection<RoomModel> RoomsInfo { get; }
    [ObservableProperty]private RoomModel? _selectedRoom;

    [RelayCommand]
    private void AddRoom()
    {
        App.GetRequiredService<AddRoomWindow>().Show();
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