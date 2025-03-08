using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;

namespace GatitoChat.Core.Security;

public static class PBCUtils
{
    //TODO: extract from device?
    private static readonly string _seedstr = "25765f57-ead2-4bc7-b21a-159941bbf087-gatito-chat";

    private static byte[] PRFExtract(byte[] data)
    {
        var key = Encoding.UTF8.GetBytes(_seedstr);
        return HMACUtils.Compute(key, data);
    }

    public static string BlindUid(string uid)
    {
        var blindUid = Convert.ToBase64String(PRFExtract(Encoding.UTF8.GetBytes(uid)));
        return blindUid;
    }
    public static (ECPrivateKeyParameters,string) KeyGen(string pw)
    {
        var data = Encoding.UTF8.GetBytes(pw+_seedstr);
        var seed = PRFExtract(data);
        
        var pair=SignUtils.GenerateKeyPair(seed);
        var privateKey = (ECPrivateKeyParameters)pair.Private;
        var publicKey=SignUtils.ExportPublicKey((ECPublicKeyParameters)pair.Public);
        return (privateKey,publicKey);
    }

    public static string Sign(ECPrivateKeyParameters privateKey, byte[] data)
    {
        var bytes = SignUtils.SignData(data, privateKey);
        Console.WriteLine("ORI SIGN: "+Convert.ToBase64String(bytes));
        var encrypted = RsaUtils.RsaEncrypt(bytes);
        return Convert.ToBase64String(encrypted);
    }

    public static string GenerateRandomString(int length)
    {
        byte[] randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}