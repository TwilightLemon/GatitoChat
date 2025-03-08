using System.Security.Cryptography;

namespace GatitoChat.Core.Security;

internal static class HMACUtils
{
    public static byte[] Compute(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }
}