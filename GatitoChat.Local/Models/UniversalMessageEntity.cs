namespace GatitoChat.Local.Models;

/// <summary>
/// Message Type for UniversalMessageEntity(Local)
/// in a local room, all message is sent by the user/client 
/// this enum is used to distinguish between user and system message.
/// </summary>
public enum MessageType{User,System}
public record class UniversalMessageEntity(MessageType Type,string Name, string Message);