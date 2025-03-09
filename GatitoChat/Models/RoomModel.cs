using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GatitoChat.Core.Models;

namespace GatitoChat.Models;

public record MessageItem(string Type,string Name,string Content);
public partial class RoomModel(string name,string hashId):ObservableObject
{
    [ObservableProperty] private string _name = name;
    [ObservableProperty] private string _hashId = hashId;
    [ObservableProperty] private string _lastMsg = "Welcome to GatitoChat"; //for demo use

    public ObservableCollection<MessageItem> Messages { get; set; } = [];
}