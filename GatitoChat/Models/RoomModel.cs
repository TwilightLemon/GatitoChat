using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GatitoChat.Core.Models;

namespace GatitoChat.Models;

public enum SenderType{Self,Other,System}
public record MessageItem(SenderType Type,string Name,string Content,string? ImageData=null);
public partial class RoomModel(string name,string hashId):ObservableObject
{
    /// <summary>
    /// Room name, used for display.
    /// </summary>
    [ObservableProperty] private string _name = name;
    /// <summary>
    /// Room ID, used for identification and message sending. (same in server and client)
    /// </summary>
    [ObservableProperty] private string _hashId = hashId;
    /// <summary>
    /// Last message. Also used for state display.
    /// </summary>
    [ObservableProperty] private string _lastMsg = "::Disconnected"; //for init use
    [ObservableProperty] private bool _isLocalRoom = false;
    public ObservableCollection<MessageItem> Messages { get; set; } = [];
}