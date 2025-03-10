using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;

namespace GatitoChat.Core;

public class AuthenticationClient(HttpClient hc,string endpoint)
{
    public async Task<LoginResponse?> CheckUser(string uid)
    {
        var blindUid = PBCUtils.BlindUid(uid);
        var msg = new LoginEntity() { Uid = blindUid, Rnd = "query" };
        var content=JsonContent.Create(msg, AppJsonContext.Default.LoginEntity);
        var response = await hc.PostAsync(endpoint + "check", content);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize(json, AppJsonContext.Default.LoginResponse);
    }
    
    public async Task<(bool,RegisterEntity?)> Register(string username, string pw,string uid)
    {
        var (_,pkSign) = PBCUtils.KeyGen(pw);
        Console.WriteLine("pkSign: "+pkSign);
        var blindUid = PBCUtils.BlindUid(uid);
        var msg = new RegisterEntity()
        {
            Name = username,
            Uid = blindUid,
            PkSign = pkSign
        };
        var content=JsonContent.Create(msg,AppJsonContext.Default.RegisterEntity);
        var response = await hc.PostAsync(endpoint+"register",content);
        var dt = JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(),AppJsonContext.Default.RegisterEntity);
        bool success = dt!=null&&dt.Uid==blindUid&&dt.PkSign==pkSign&&dt.Name==username;
        return (success,dt);
    }
    private static string Sign(string pw, string uid)
    {
        var (privateKey, pk) = PBCUtils.KeyGen(pw);
        var blindUid = PBCUtils.BlindUid(uid);
        var data = Encoding.UTF8.GetBytes(blindUid);
        return PBCUtils.Sign(privateKey, data);
    }

    public async Task<UserCredential?> Login(string uid, string pw)
    {
        var sign = Sign(pw, uid);
        var blindUid = PBCUtils.BlindUid(uid);
        var randomSeed = PBCUtils.GenerateRandomString(32);
        var msg = new LoginEntity()
        {
            Sign = sign,
            Uid = blindUid,
            Rnd = randomSeed
        };
        var content = JsonContent.Create(msg, AppJsonContext.Default.LoginEntity);
        var response = await hc.PostAsync(endpoint+"login",content);
        var result= JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(), AppJsonContext.Default.LoginResponse);
        if (result is { UserName: not null, Token: not null })
            return new(result.UserName, blindUid, result.Token, randomSeed, sign);
        return null;
    }
}