using System.Security.Cryptography;
using System.Text;

namespace GatitoChat.Core.Security;

public static class MD5Utils
{
    public static string Hash(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = MD5.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}