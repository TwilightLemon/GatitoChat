using System.Security.Cryptography;
using System.Text;

namespace GatitoChat.Core.Security;

public class AesUtils
{
    //相信伟大的AOT编译会保护它(bushi)
    private static readonly string _embeddedKeySeed = "fadf1eb8-c47f-47c9-b812-fc263e42655b-gatito-chat";
    private static readonly byte[] _embeddedKey = GetHash128(_embeddedKeySeed);

    internal static byte[] GetHash128(string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] hashBytes = MD5.HashData(keyBytes);
        return hashBytes;
    }

    internal static byte[] DeriveKey(string key)
    {
        byte[] addi = GetHash128(key);
        //addi xor with the embedded key
        for (int i = 0; i < _embeddedKey.Length; i++)
            addi[i]  ^= _embeddedKey[i];
        return addi;
    }

    public static string Encrypt(string msg,string key)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(msg);
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = DeriveKey(key);
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.Padding = PaddingMode.PKCS7;

        //生成随机IV，最后将IV拷贝到result数组的前16个字节
        aesAlg.GenerateIV();
        byte[] iv = aesAlg.IV;
        
        using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using var bwEncrypt = new BinaryWriter(csEncrypt);
            bwEncrypt.Write(dataToEncrypt);
        }

        byte[] encryptedData = msEncrypt.ToArray();
        byte[] result = new byte[iv.Length + encryptedData.Length];
        //将IV和加密后的数据拷贝到result数组
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, 0, result, iv.Length, encryptedData.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string base64CipherText,string key)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(base64CipherText);
        
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = DeriveKey(key);
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.Padding = PaddingMode.PKCS7;

        //从dataToDecrypt数组中分离出IV和密文
        byte[] iv = new byte[aesAlg.BlockSize / 8];
        byte[] cipherText = new byte[dataToDecrypt.Length - iv.Length];
        Buffer.BlockCopy(dataToDecrypt, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(dataToDecrypt, iv.Length, cipherText, 0, cipherText.Length);

        aesAlg.IV = iv;

        using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var brDecrypt = new BinaryReader(csDecrypt);
        byte[] decryptedData = brDecrypt.ReadBytes(cipherText.Length);
        
        return Encoding.UTF8.GetString(decryptedData);
    }
}