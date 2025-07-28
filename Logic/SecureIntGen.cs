using System;
using System.Security.Cryptography;

namespace DiceGame.Utils
{
    public sealed class SecureIntGen : IDisposable
    {
        private readonly HmacGenerator _hmac = new();
        private readonly int _min, _max;

        public SecureIntGen(int min, int max) => (_min, _max) = (min, max);
        
        public (byte[] key, byte[] hmac, int value) GenerateCommitment()
        {
            var value = RandomNumberGenerator.GetInt32(_min, _max);
            var (key, hmac) = _hmac.GenerateProof(value);
            return (key, hmac, value);
        }

        public void Dispose() => _hmac.Dispose();
    }
}