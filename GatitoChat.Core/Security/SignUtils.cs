using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace GatitoChat.Core.Security;

public static class SignUtils
{
    public static AsymmetricCipherKeyPair GenerateKeyPair(byte[] seed)
    {
        var random = new SecureRandom(new FixedRandomGenerator(seed));
        var generator = new ECKeyPairGenerator("ECDSA");
        
        var keyGenParams = new KeyGenerationParameters(
            random, 
            256
        );
        
        generator.Init(keyGenParams);
        return generator.GenerateKeyPair();
    }
    
    public static byte[] SignData(byte[] data, ECPrivateKeyParameters privateKey)
    {
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(data, 0, data.Length);
        return signer.GenerateSignature();
    }
    
    public static string ExportPublicKey(ECPublicKeyParameters publicKey)
    {
        // 关键修改：使用 IdECPublicKey 作为算法标识符
        var algorithmIdentifier = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, publicKey.PublicKeyParamSet);

        // 公钥点 Q 的编码（未压缩格式）
        var q = publicKey.Q.GetEncoded(false); // false 表示未压缩格式

        var ecPublicKey = new SubjectPublicKeyInfo(
            algorithmIdentifier,  // 正确标识公钥类型
            new DerBitString(q)    // 公钥数据
        );

        // 转换为 PEM 格式
        return "-----BEGIN PUBLIC KEY-----\n" +
               Convert.ToBase64String(ecPublicKey.GetDerEncoded(), Base64FormattingOptions.InsertLineBreaks) +
               "\n-----END PUBLIC KEY-----";
    }
}