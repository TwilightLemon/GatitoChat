// See https://aka.ms/new-console-template for more information

using GatitoChat.Core;
using GatitoChat.Core.Security;

string pw = "123456", name = "twlmGatito", uid = "aaa@example.com";

string endpoint = "http://127.0.0.1:3000/user/";
Console.WriteLine("Hello, World!");

Console.WriteLine(MD5Utils.Hash(endpoint));



//var auth=new AuthenticationClient(new(),endpoint);
/*var res=await auth.Register(name,pw,uid);
Console.WriteLine(res);*/

/*var resQ=await auth.Login(uid,pw);
if (resQ is { Success: true } result)
{
    Console.WriteLine(result.UserName);
}*/

/*var (_, pk1) = PBCUtils.KeyGen(pw);
Console.WriteLine("PK1: "+pk1);

var (_, pk) = PBCUtils.KeyGen(pw);
Console.WriteLine("PK2: "+pk);*/