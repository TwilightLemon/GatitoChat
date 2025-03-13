using Fleck;
namespace GatitoChat.Local;

//a local server only supports single chat room.(lazy)
public class WsServer(int port):IDisposable
{
    private readonly WebSocketServer _wsServer = new($"ws://127.0.0.1:{port}")
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
                _sockets.ForEach(s => {
                    if (s.IsAvailable&&s!=socket)
                        s.Send(message); 
                });
            };
        });
    }
    
}