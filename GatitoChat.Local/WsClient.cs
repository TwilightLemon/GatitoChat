﻿using GatitoChat.Local.Models;
using GatitoChat.Core.Security;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GatitoChat.Local;

/// <summary>
/// local client for local server
/// </summary>
public class WsClient:IDisposable
{
    private ClientWebSocket? _ws;
    private Task? _receiveTask;
    private CancellationTokenSource? _cts;
    public event Action<UniversalMessageEntity>? OnMessageReceived;
    public event Action? OnConnectSucceeded, OnConnectFailed;
    public int ConnectionPort { get; set; }
    private string _aesAddiKey => $"port:{ConnectionPort}";
    public async Task<bool> ConnectAsync(string ip,int port)
    {
        try
        {
            string connectionUri = $"ws://{ip}:{port}";
            _ws = new();
            ConnectionPort = port;
            await _ws.ConnectAsync(new Uri(connectionUri),CancellationToken.None);
            if (_ws.State != WebSocketState.Open)
            {
                OnConnectFailed?.Invoke();
                return false;
            }
            else OnConnectSucceeded?.Invoke();
        }
        catch
        {
            OnConnectFailed?.Invoke();
            return  false;
        }
        _cts = new();
        _receiveTask = ReceiveMessage(_cts.Token);
        return true;
    }

    private async Task ReceiveMessage(CancellationToken cancellationToken)
    {
        if (_ws == null) return;
        var buffer = new byte[1024 * 4];
        var sb = new StringBuilder();
        while (_ws.State == WebSocketState.Open&& !cancellationToken.IsCancellationRequested)
        {
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                OnConnectFailed?.Invoke();
                break;
            }
            var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            sb.Append(msg);
            if (result.EndOfMessage)
            {
                //by contrast, local server sends system message in cyphertext (the user sends it, actually)
                var plainText = AesUtils.Decrypt(sb.ToString(),_aesAddiKey);
                var msgEntity = JsonSerializer.Deserialize(plainText, AppJsonContext.Default.UniversalMessageEntity);
                if (msgEntity != null)
                    OnMessageReceived?.Invoke(msgEntity);
                sb.Clear();
            }
        }
    }

    public async Task SendMessage(MessageType type,string name,string msg)
    {
        if (_ws == null) return;
        if (_ws.State == WebSocketState.Open)
        {
            var plainText=JsonSerializer.Serialize(new UniversalMessageEntity(type,name, msg),AppJsonContext.Default.UniversalMessageEntity);
            var cipherText =AesUtils.Encrypt(plainText,_aesAddiKey);
            var bytes=Encoding.UTF8.GetBytes(cipherText);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts!.Token);
        }
        else
        {
            OnConnectFailed?.Invoke();
        }
    }
    public Task JoinRoom(string name)
        =>SendMessage(MessageType.System,name,$"{name} has joined this room.");

    public Task LeaveRoom(string name)
        =>SendMessage(MessageType.System,name,$"{name} has left this room.");

    public void Dispose()
    {
        _ws?.Dispose();
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
