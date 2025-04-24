using GatitoChat.Core;
using GatitoChat.Local;
using GatitoChat.Local.Models;
using GatitoChat.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace GatitoChat.Services;

public class LocalChatService(ChatClientService chatClientService)
{
    private WsServer? _server;
    private WsClient? _client;
    private readonly ObservableCollection<RoomModel> _rooms = chatClientService.Rooms;
    private RoomModel? _currentRoom; // only single local room allowed.
    /// <summary>
    /// username as an identity
    /// </summary>
    private string  _nickname=string.Empty;
    
    public async Task<bool> LaunchServer(int port, string nickname)
    {
        try
        {
            if (_server != null) return false;

            _server = new(port);
            _server.Start();

            //server also needs to join the room
            return await JoinLocalRoom("127.0.0.1", port, nickname);
        }
        catch
        {
            CloseServer();
            return false;
        }
    }

    private void CloseServer()
    {
        if (_server == null) return;
        _server.Dispose();
        _server = null;
    }

    public async Task<bool> JoinLocalRoom(string ip,int port, string nickname)
    {
        if( _client != null ) return false;
        
        string uri=$"ws://{ip}:{port}";
        _client=new();
        _client.OnMessageReceived += Client_OnMessageReceived;
        var success = await _client.ConnectAsync(uri);
        if (success)
        {
            _currentRoom = new(uri, "")// for a local room, there is no room name or hashId.
            {
                IsLocalRoom = true,LastMsg = "::LocalRoom"
            };
            _rooms.Add(_currentRoom);
            _nickname = nickname;
            //JOIN ROOM MESSAGE
            await _client.JoinRoom(nickname);
            return true;
        }
        _client.Dispose();
        _client = null;
        return false;
    }

    private void Client_OnMessageReceived(UniversalMessageEntity message)
    {
        if (_currentRoom == null) return;

        //the local chat server will not resend the message to the sender, so received message is always from the other.
        // and also the local chat server only supports user message.
        if (message.Type == MessageType.System)
        {
            _currentRoom.Messages.Add(new(SenderType.System, message.Name, message.Message));
            _currentRoom.LastMsg = $"{message.Name}: {message.Message}";
        }
        else
        {
            var msgBody = JsonSerializer.Deserialize(message.Message, AppJsonContext.Default.MessageContent);
            if(msgBody==null) return;
            var displayText= msgBody.Type == ContentType.PlainText ? msgBody.Content : "[Image]";
            var imageData=msgBody.Type == ContentType.Image ? msgBody.Content : null;
            _currentRoom.Messages.Add(new(SenderType.Other, message.Name, displayText,imageData));
            _currentRoom.LastMsg = $"{message.Name}: {displayText}";
        }

    }

    public async Task SendMessageAsync(string message)
    {
        if (_client != null&&_currentRoom!=null)
        {
            var msg=new MessageContent(ContentType.PlainText,message);
            var msgBody = JsonSerializer.Serialize(msg, AppJsonContext.Default.MessageContent);
            await _client.SendMessage(MessageType.User,_nickname, msgBody);
            _currentRoom.Messages.Add(new (SenderType.Self,_nickname,message));
        }
    }

    public async Task SendImageAsync(string base64)
    {
        if (_client != null && _currentRoom != null)
        {
            var msg = new MessageContent(ContentType.Image, base64);
            var msgBody = JsonSerializer.Serialize(msg, AppJsonContext.Default.MessageContent);
            await _client.SendMessage(MessageType.User, _nickname, msgBody);

            _currentRoom.Messages.Add(new(SenderType.Self, _nickname, "[Image]",base64));
        }
    }

    public async Task<bool> LeaveLocalRoom()
    {
        try
        {
            if (_client == null || _currentRoom == null) return false;
            await _client.LeaveRoom(_nickname);
            _client.Dispose();
            _client = null;
            _rooms.Remove(_currentRoom);
            _currentRoom = null;
            CloseServer();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    
}