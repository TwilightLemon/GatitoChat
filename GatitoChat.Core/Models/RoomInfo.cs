namespace GatitoChat.Core.Models;

public sealed class RoomInfo(string name,string hash)
{
    public string Name { get; } = name;
    public string Id { get; } = hash;
}