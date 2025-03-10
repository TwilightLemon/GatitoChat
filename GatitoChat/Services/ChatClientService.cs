using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GatitoChat.Core;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;
using GatitoChat.Models;

namespace GatitoChat.Services;

public class ChatClientService:IAsyncDisposable
{
    private readonly UserProfileService _userProfileService;
    private readonly ChatClient _chatClient = new();
    public ObservableCollection<RoomModel> Rooms { get; } = [];

    public ChatClientService(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
        _userProfileService.OnLoginCallback += UserProfileService_OnLogin;
        _chatClient.OnMessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(MessageResponse msg)
    {
        if (Rooms.FirstOrDefault(r => r.HashId == msg.RoomId) is { } room)
        {
            var type = SenderType.Other;
            if (msg.Type is MessageType.Join or MessageType.Leave)
            {
                type = SenderType.System;
            }else if (msg.Type is MessageType.Chat && msg.SenderId==_userProfileService.Credential!.BlindedUid)
            {
                type = SenderType.Self;
            }
            room.Messages.Add(new MessageItem(type,msg.SenderName,msg.Message));
            room.LastMsg=$"{msg.SenderName}: {msg.Message}";
        }
    }

    private async void UserProfileService_OnLogin()
    {
        _chatClient.UserInfo = _userProfileService.Credential!;
        await _chatClient.Connect(_userProfileService.ChatServerUri);
    }

    public async Task JoinRoom(string roomName)
    {
        var hashId = MD5Utils.Hash(roomName);
        await _chatClient.JoinRoom(hashId);
        Rooms.Add(new(roomName,hashId));
    }

    public async Task LeaveRoom(RoomModel room)
    {
        await _chatClient.LeaveRoom(room.HashId);
        Rooms.Remove(room);
    }

    public async Task SendMessage(RoomModel room,string message)
    {
        await _chatClient.ChatMessage(room.HashId,message);
    }

    public async ValueTask DisposeAsync()
    {
        await _chatClient.Close(Rooms.Select(r=>r.HashId).ToArray());
        _chatClient.Dispose();
    }
}