using System.Net.Http.Json;
using System.Text;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;

namespace GatitoChat.Core;

public class AuthenticationClient(HttpClient hc,string endpoint)
{
    public async Task<LoginResponse?> CheckUser(string uid)
    {
        var blindUid = PBCUtils.BlindUid(uid);
        var response = await hc.PostAsJsonAsync(endpoint + "check", new LoginEntity(){Uid = blindUid,Rnd = "query"});
        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
    
    public async Task<(bool,RegisterEntity?)> Register(string username, string pw,string uid)
    {
        var (_,pkSign) = PBCUtils.KeyGen(pw);
        Console.WriteLine("pkSign: "+pkSign);
        var blindUid = PBCUtils.BlindUid(uid);
        var response = await hc.PostAsJsonAsync(endpoint+"register", new RegisterEntity()
        {
            Name = username,
            Uid = blindUid,
            PkSign = pkSign
        });
        var dt = await response.Content.ReadFromJsonAsync<RegisterEntity>();
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
        var response = await hc.PostAsJsonAsync(endpoint+"login", new LoginEntity()
        {
            Sign = sign,Uid=blindUid,Rnd =randomSeed
        });
        var result= await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result is { UserName: not null, Token: not null })
            return new(result.UserName, blindUid, result.Token, randomSeed, sign);
        return null;
    }
}