using Org.BouncyCastle.Crypto.Prng;

namespace GatitoChat.Core.Security;

public class FixedRandomGenerator(byte[] seed) : IRandomGenerator
{
    private readonly byte[] _seed = (byte[])seed.Clone(); // 防止外部修改
    private int _position = 0;

    public void AddSeedMaterial(byte[] seed)
    {
        // ignore
    }

    public void AddSeedMaterial(ReadOnlySpan<byte> seed)
    {
        //
    }

    public void AddSeedMaterial(long seed)
    {
        // ignore
    }

    public void NextBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = _seed[_position % _seed.Length];
            _position++;
        }
    }

    public void NextBytes(Span<byte> bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = _seed[_position % _seed.Length];
            _position++;
        }
    }

    public void NextBytes(byte[] bytes, int start, int len)
    {
        for (int i = 0; i < len; i++)
        {
            bytes[start + i] = _seed[_position % _seed.Length];
            _position++;
        }
    }
}