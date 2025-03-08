using GatitoChat.Core;

namespace GatitoChat.Services;

public class CommonClientService
{
    public RoomManager? RoomManager { get; set; }
    public ChatClient? ChatClient { get; set; }
}