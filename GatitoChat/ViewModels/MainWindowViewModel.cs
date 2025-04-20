using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GatitoChat.Core.Models;
using GatitoChat.Models;
using GatitoChat.Services;
using GatitoChat.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GatitoChat.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly UserProfileService _userProfileService;
    private readonly ChatClientService _chatClientService;
    private readonly LocalChatService _localChatService;

    public MainWindowViewModel(UserProfileService userProfileService,
        ChatClientService chatClientService,
        LocalChatService localChatService)
    {
        _userProfileService = userProfileService;
        _chatClientService = chatClientService;
        _localChatService = localChatService;
        RoomsInfo = chatClientService.Rooms;
        chatClientService.OnConnectionFailed += ChatClientService_OnConnectionFailed;
        chatClientService.OnConnectionSucceeded += ChatClientService_OnConnectionSucceeded;
        userProfileService.OnLoginCallback += UserProfileService_OnLoginCallback;
    }

    [ObservableProperty] private bool _isConnectionFailed = false;//usually assume connection succeed. 
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

    [ObservableProperty] private string _loginBtnHint = "Login";
    [RelayCommand]
    private void Login()
    {
        App.GetRequiredService<LoginWindow>().ShowDialog(App.MainWindow);
    }

    /// <summary>
    /// 指示Room列表的引用，实体由ChatClientService创建(无论是remote/local room)
    /// </summary>
    public ObservableCollection<RoomModel> RoomsInfo { get; }
    [ObservableProperty] private RoomModel? _selectedRoom;

    [RelayCommand]
    private void AddRoom()
    {
        App.GetRequiredService<AddRoomWindow>().ShowDialog(App.MainWindow);
    }

    [RelayCommand]
    private async Task LeaveRoom()
    {
        if (SelectedRoom == null) return;
        if (SelectedRoom.IsLocalRoom)
            await _localChatService.LeaveLocalRoom();
        else await _chatClientService.LeaveRoom(SelectedRoom);

        SelectedRoom = null;
    }

    [ObservableProperty] private string _message = "";

    [RelayCommand]
    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(Message) || SelectedRoom == null) return;
        string msg = Message;
        if (SelectedRoom.IsLocalRoom)
            await _localChatService.SendMessageAsync(msg);
        else await _chatClientService.SendMessage(SelectedRoom, msg);
        Message = "";
    }

    [RelayCommand]
    private async Task SendImage()
    {
        if (SelectedRoom == null) return;
        var files = await App.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select A Image File",
            AllowMultiple = false
        });

        if (files.Count == 1)
        {
            var file = files[0];
            var filePath = file.TryGetLocalPath();
            if (filePath == null) return;
            if ((await file.GetBasicPropertiesAsync()).Size > 1024 * 1024 * 5)
            {
                MessageBox.Show("This file is too large! within 5MB please.");
                return;
            }

            var base64 = Convert.ToBase64String(File.ReadAllBytes(filePath));
            if (SelectedRoom.IsLocalRoom)
            {
                await _localChatService.SendImageAsync(base64);
            }
            else
            {
                await _chatClientService.SendImage(SelectedRoom, base64);
            }
        }
    }
}