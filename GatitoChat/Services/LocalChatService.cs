using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GatitoChat.Local;
using GatitoChat.Local.Models;
using GatitoChat.Models;

namespace GatitoChat.Services;

public class LocalChatService(ChatClientService chatClientService)
{
    private WsServer? _server;
    private WsClient? _client;
    private readonly ObservableCollection<RoomModel> _rooms = chatClientService.Rooms;
    private RoomModel? _currentRoom;
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
        
        //the local chat server will not resend the message to the sender, so received message is always from other.
        // and also the local chat server only supports user message.
        _currentRoom.Messages.Add(new (message.Type==MessageType.User?SenderType.Other:SenderType.System,message.Name,message.Message));
        _currentRoom.LastMsg = $"{message.Name}: {message.Message}";
    }

    public async Task SendMessageAsync(string message)
    {
        if (_client != null&&_currentRoom!=null)
        {
            await _client.SendMessage(MessageType.User,_nickname, message);
            _currentRoom.Messages.Add(new (SenderType.Self,_nickname,message));
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