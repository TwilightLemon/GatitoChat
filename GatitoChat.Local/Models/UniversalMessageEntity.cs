namespace GatitoChat.Local.Models;

public enum MessageType{User,System}
public record class UniversalMessageEntity(MessageType Type,string Name, string Message);