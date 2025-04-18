using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GatitoChat.Core.Models;
using GatitoChat.Core.Security;

namespace GatitoChat.Core;

public class AuthenticationClient(HttpClient hc,string endpoint)
{
    public async Task<CheckResponse?> CheckUser(string uid,CancellationToken cancellationToken)
    {
        var blindUid = PBCUtils.BlindUid(uid);
        var msg = new LoginEntity() { Uid = blindUid, Rnd = Guid.NewGuid().ToString() };
        var content=JsonContent.Create(msg, AppJsonContext.Default.LoginEntity);
        var response = await hc.PostAsync(endpoint + "check", content, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var res= JsonSerializer.Deserialize(json, AppJsonContext.Default.CheckResponse);
        if (res != null){
            res.rnd = msg.Rnd;
            res.Uid = blindUid;
        }
        return res;
    }

    public async Task<VerifierResponse?>  VerifyEmail(CheckResponse check,string email,CancellationToken cancellationToken)
    {
        if (check is not { rnd: not null, SessionId: not null,Uid:not null}) return null;
        var msg = new VerifierEntity()
        {
            Email = email,
            BlindedUid = check.Uid,
            Rnd = check.rnd,
            SessionId = check.SessionId
        };
        var content = JsonContent.Create(msg, AppJsonContext.Default.VerifierEntity);
        var response = await hc.PostAsync(endpoint + "code", content, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize(json, AppJsonContext.Default.VerifierResponse);
    }
    
    public async Task<(bool,RegisterEntity?)> Register(CheckResponse check,VerifierResponse verification,
            string oriCode,string username, string pw,string uid,CancellationToken cancellationToken)
    {
        if (check is not { rnd: not null, SessionId: not null }) return (false, null);

        var (_,pkSign) = PBCUtils.KeyGen(pw);
        Console.WriteLine("pkSign: "+pkSign);
        var blindUid = PBCUtils.BlindUid(uid);
        var msg = new RegisterEntity()
        {
            Name = username,
            Uid = blindUid,
            PkSign = pkSign,
            Rnd=check.rnd,
            SessionId=check.SessionId,
            VfCode=verification.Code,
            OriVfCode=oriCode
        };
        var content=JsonContent.Create(msg,AppJsonContext.Default.RegisterEntity);
        var response = await hc.PostAsync(endpoint + "register", content, cancellationToken);
        var dt = JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(cancellationToken), AppJsonContext.Default.RegisterEntity);
        bool success = dt!=null&&dt.Uid==blindUid&&dt.PkSign==pkSign&&dt.Name==username;
        return (success,dt);
    }
    private static string Sign(string pw, string uid)
    {
        var (privateKey, _) = PBCUtils.KeyGen(pw);
        var blindUid = PBCUtils.BlindUid(uid);
        var data = Encoding.UTF8.GetBytes(blindUid);
        return PBCUtils.Sign(privateKey, data);
    }

    public async Task<UserCredential?> Login(string uid, string pw,CancellationToken cancellationToken)
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
        var response = await hc.PostAsync(endpoint + "login", content, cancellationToken);
        var result= JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(cancellationToken), AppJsonContext.Default.LoginResponse);
        if (result is { UserName: not null, Token: not null })
            return new(result.UserName, blindUid, result.Token, randomSeed, sign);
        return null;
    }
}