using Fleck;
namespace GatitoChat.Local;

/// <summary>
/// a local server only supports single chat room. (lazy)
/// </summary>
/// <param name="port"></param>
public class WsServer(int port):IDisposable
{
    private readonly WebSocketServer _wsServer = new($"ws://0.0.0.0:{port}")//使用默认路由而不是回环
    {
        RestartAfterListenError = true
    };
    private readonly List<IWebSocketConnection> _sockets = [];

    public void Dispose()
    {
        _wsServer.Dispose();
    }

    public void Start()
    {
        _wsServer.Start(socket =>
        {
            socket.OnOpen = () => { 
                _sockets.Add(socket);
            };
            socket.OnClose = () => {
                _sockets.Remove(socket);
            };
            socket.OnMessage = message =>
            {
                //broadcast to all other clients
                _sockets.ForEach(s => {
                    if (s.IsAvailable&&s!=socket)//exclude self
                        s.Send(message); 
                });
            };
        });
    }
    
}