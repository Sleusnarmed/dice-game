using System.Security.Cryptography;

public class HmacGenerator : IDisposable
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    
    public (byte[] key, byte[] hmac) GenerateProof(int value)
    {
        byte[] key = new byte[32];
        _rng.GetBytes(key);
        
        using var hmac = new HMACSHA3_256(key);
        return (key, hmac.ComputeHash(BitConverter.GetBytes(value)));
    }

    public void Dispose() => _rng.Dispose();
}