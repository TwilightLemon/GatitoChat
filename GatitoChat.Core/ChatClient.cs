using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GatitoChat.Core.Models;

namespace GatitoChat.Core;

public class ChatClient(string connectionUri,LoginResponse userInfo)
{
    private ClientWebSocket? ws;
    private Task? receiveTask;
    public event Action? OnConnectionFailed, OnConnectionSuccess, OnConnectionClosed;
    public delegate void MessageReceivedHandler(MessageResponse msg);
    public event MessageReceivedHandler? OnMessageReceived;
    public async Task Connect()
    {
        var uri = new Uri(connectionUri);
        ws = new();
        await ws.ConnectAsync(uri, CancellationToken.None);
        if (ws.State != WebSocketState.Open)
        {
            OnConnectionFailed?.Invoke();
            return;
        }
        else OnConnectionSuccess?.Invoke();

        receiveTask = ReceiveMessage();
    }
    private static string StringifyMsgBody(MessageEntity body)
        =>JsonSerializer.Serialize(body,AppJsonContext.Default.MessageEntity);
    private static string StringifyExitMsgBody(ExitEntity body)
        =>JsonSerializer.Serialize(body,AppJsonContext.Default.ExitEntity);
    private async Task ReceiveMessage()
    {
        if (ws == null) return;
        var buffer = new byte[1024 * 4];
        var sb = new StringBuilder();
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                //the server rejected.
                break;
            }
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            sb.Append(msg);
            if (result.EndOfMessage)
            {
                string total = sb.ToString();
                var body=JsonSerializer.Deserialize(total, AppJsonContext.Default.MessageResponse);
                OnMessageReceived?.Invoke(body);
                sb.Clear();
            }
        }
    }

    private async Task SendMessage(string roomHash,MessageType type,string message)
    {
        if (ws == null) return;
        if (ws.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(StringifyMsgBody(new()
            {
                Name = userInfo.UserName,Token = userInfo.Token,RoomHash = roomHash,Type = type,Message = message
            }));
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public Task ChatMessage(string roomHash, string message)
        =>SendMessage(roomHash,MessageType.Chat, message);
    public Task JoinRoom(string roomHash)
        =>SendMessage(roomHash,MessageType.Join,"");

    public Task LeaveRoom(string roomHash)
        => SendMessage(roomHash, MessageType.Leave, "");

    public async Task Close(string[] roomHashes)
    {
        if (ws == null) return;
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    StringifyExitMsgBody(new()
                    {
                        Name = userInfo.UserName,Token = userInfo.Token,RoomIds = roomHashes
                    }),
                    CancellationToken.None);
        ws.Dispose();
        OnConnectionClosed?.Invoke();
    }
}
