using System.Net.Http.Json;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;

namespace GatitoChat.Core;

public class RoomManager(HttpClient hc,string endpoint)
{
    public LoginResponse? UserCredential { get; set; }

    private async Task<ActionResponse?> DoAction(string action)
    {
        if(UserCredential==null)return null;
        var res = await hc.PostAsJsonAsync(endpoint + action,new VerificationEntity()
        {
            Uid = UserCredential.Uid,Sign = UserCredential.Sign,RandomSeed = UserCredential.RandomSeed,Token = UserCredential.Token
        });
        return await res.Content.ReadFromJsonAsync<ActionResponse>();
    }

    public  Task<ActionResponse?> Connect()
        => DoAction("connect");

    public Task<ActionResponse?> Disconnect()
        =>DoAction("disconnect");

    private async Task<ActionResponse?> ManageRoom(string roomName,string operation)
    {
        if(UserCredential?.Token==null)return null;
        
        string room=MD5Utils.Hash(roomName);
        var res = await hc.PostAsJsonAsync(endpoint + operation,new RoomManageEntity()
        {
            Token = UserCredential.Token,Room = room
        });
        var result= await res.Content.ReadFromJsonAsync<ActionResponse>();
        if (result is {Success:true}) result.Message = room;
        return result;
    }

    public Task<ActionResponse?> CreateRoom(string roomName)
        => ManageRoom(roomName, "createRoom");
    public Task<ActionResponse?> CheckRoom(string roomName)
        => ManageRoom(roomName, "checkRoom");
}