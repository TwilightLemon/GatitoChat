using System.Security.Cryptography;
using System.Text;

namespace GatitoChat.Core.Security;

public static class MD5Utils
{
    public static string Hash(string data)
    {
        using var md5 = MD5.Create();
        var bytes=Encoding.UTF8.GetBytes(data);
        var hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "");
    }
}