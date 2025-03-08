using System.Reflection;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace GatitoChat.Core.Security;

public static class RsaUtils
{
    private static readonly string _publicKeyPath="GatitoChat.Core.Resources.public_key.pem";
    private static OaepEncoding? _rsaEngine;
    private static RsaKeyParameters LoadRsaPublicKey()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(_publicKeyPath);
        using var reader = new StreamReader(stream);
        var pemReader = new PemReader(reader);
        return (RsaKeyParameters)pemReader.ReadObject();
    }

    public static byte[] RsaEncrypt(byte[] data)
    {
        if(_rsaEngine==null)
        {
            _rsaEngine = new OaepEncoding(new RsaEngine(),new Sha256Digest());
            _rsaEngine.Init(true,LoadRsaPublicKey());
        }
        return _rsaEngine.ProcessBlock(data, 0, data.Length);
    }
}