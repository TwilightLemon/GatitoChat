using GatitoChat.Core;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;
using GatitoChat.Models;
using GatitoChat.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GatitoChat.Services;

public class ChatClientService
{
    private readonly UserProfileService _userProfileService;
    private readonly ChatClient _chatClient = new();

    public ObservableCollection<RoomModel> Rooms { get; } = [];
    public Action? OnConnectionFailed,OnConnectionSucceeded;
    public event Action<RoomModel,MessageItem>? OnNewMessageReceived;

    public ChatClientService(UserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
        _userProfileService.OnLoginCallback += UserProfileService_OnLogin;
        _chatClient.OnMessageReceived += OnMessageReceived;
        _chatClient.OnConnectionFailed += delegate { OnConnectionFailed?.Invoke(); };
        _chatClient.OnConnectionSucceeded += delegate { OnConnectionSucceeded?.Invoke(); };
    }

    private void OnMessageReceived(MessageResponse msg)
    {
        if (Rooms.FirstOrDefault(r => r.HashId == msg.RoomId) is { } room)
        {
            var type = SenderType.Other;
            if (msg.Type is MessageType.Join or MessageType.Leave) //system message
            {
                type = SenderType.System;
            }
            else if (msg.Type is MessageType.Chat &&
            msg.SenderId==_userProfileService.Credential!.BlindedUid) //self message. Note that the server will also resend the message to the sender.
            {
                type = SenderType.Self;
            }

            var (displayText,imageData)=MessageContent.Parse(msg, type);
            if (displayText == null) return;

            var msgItem = new MessageItem(type, msg.SenderName, displayText,imageData);
            room.Messages.Add(msgItem);
            //update last message
            room.LastMsg=$"{msg.SenderName}: {displayText}";
            OnNewMessageReceived?.Invoke(room,msgItem);
        }
    }

    private async void UserProfileService_OnLogin()
    {
        //login success, connect to server
        _chatClient.UserInfo = _userProfileService.Credential!;
        await _chatClient.ConnectAsync(_userProfileService.ChatServerUri);
    }
    
    public Task ReConnect()=>_chatClient.ConnectAsync(_userProfileService.ChatServerUri);

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
        var msg = new MessageContent(ContentType.PlainText, message);
        var msgBody = JsonSerializer.Serialize(msg, AppJsonContext.Default.MessageContent);
        await _chatClient.ChatMessage(room.HashId,msgBody);
    }

    public async Task SendImage(RoomModel room,string base64)
    {
        var msg = new MessageContent(ContentType.Image, base64);
        var msgBody = JsonSerializer.Serialize(msg, AppJsonContext.Default.MessageContent);
        await _chatClient.ChatMessage(room.HashId, msgBody);
    }

    public async Task CloseAll()
    {
        await _chatClient.Close([.. Rooms.Select(r=>r.HashId)]);
        _chatClient.Dispose();
    }
}