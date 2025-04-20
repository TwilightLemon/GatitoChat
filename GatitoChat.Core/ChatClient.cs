using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;

namespace GatitoChat.Core;

public class ChatClient:IDisposable
{
    private ClientWebSocket? ws;
    private Task? receiveTask;
    private CancellationTokenSource? _cts;
    public UserCredential UserInfo { get; set; }
    public event Action? OnConnectionFailed, OnConnectionSucceeded;
    public delegate void MessageReceivedHandler(MessageResponse msg);
    public event MessageReceivedHandler? OnMessageReceived;
    public async Task ConnectAsync(string connectionUri)
    {
        try
        {
            var uri = new Uri(connectionUri);
            ws = new();
            await ws.ConnectAsync(uri, CancellationToken.None);
            if (ws.State != WebSocketState.Open)
            {
                OnConnectionFailed?.Invoke();
                return;
            }
            else OnConnectionSucceeded?.Invoke();
        }
        catch
        {
            OnConnectionFailed?.Invoke();
            return;
        }
        //connect successfully, begin to receive data.
        _cts = new();
        receiveTask = ReceiveMessage(_cts.Token);
    }
    private static string StringifyMsgBody(MessageEntity body)
        =>JsonSerializer.Serialize(body,AppJsonContext.Default.MessageEntity);
    private static string StringifyExitMsgBody(ExitEntity body)
        =>JsonSerializer.Serialize(body,AppJsonContext.Default.ExitEntity);
    private async Task ReceiveMessage(CancellationToken cancellationToken)
    {
        if (ws == null) return;
        var buffer = new byte[1024 * 4];
        var sb = new StringBuilder();
        while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                //the server rejected. (usually for invalid token)
                OnConnectionFailed?.Invoke();
                break;
            }
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            sb.Append(msg);
            if (result.EndOfMessage)
            {
                string total = sb.ToString();
                var body=JsonSerializer.Deserialize(total, AppJsonContext.Default.MessageResponse);
                if (body is not null)
                {
                    if (body.Type == MessageType.Chat)
                    {
                        body.Message = AesUtils.Decrypt(body.Message);
                    }
                    OnMessageReceived?.Invoke(body);
                }

                sb.Clear();
            }
        }
    }

    private async Task SendMessage(string roomHash,string type,string message)
    {
        if (ws == null) return;
        if (ws.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(StringifyMsgBody(new()
            {
                Name = UserInfo.Username,
                SenderId = UserInfo.BlindedUid,
                Token = UserInfo.Token,
                RoomHash = roomHash,
                Type = type,CipherMsg = message,
                Sign = UserInfo.Sign
            }));
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,_cts!.Token);
        }
        else
        {
            OnConnectionFailed?.Invoke();
        }
    }

    public Task ChatMessage(string roomHash, string message)
        =>SendMessage(roomHash,MessageType.Chat, AesUtils.Encrypt(message));
    public Task JoinRoom(string roomHash)
        =>SendMessage(roomHash,MessageType.Join,"");
        // send empty message to join room, if the room is not exist, the server will create it automatically.

    public Task LeaveRoom(string roomHash)
        => SendMessage(roomHash, MessageType.Leave, "");

    public async Task Close(string[] roomHashes)
    {
        if (ws == null) return;
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    StringifyExitMsgBody(new()
                    {
                        Name = UserInfo.Username,RoomIds = roomHashes
                    }),
                    _cts!.Token);
        ws.Dispose();
    }

    public void Dispose()
    {
        ws?.Dispose();
        receiveTask?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
