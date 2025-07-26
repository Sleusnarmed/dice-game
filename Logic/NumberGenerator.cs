using System.Net.Http;
using System.Threading.Tasks;

namespace DiceGame.Utils;

public interface INumberGenerator : IDisposable
{
    Task<(byte[] key, byte[] hmac, int value)> GenerateCommitmentAsync();
}

public class NumberGenerator : HmacGenerator, INumberGenerator
{
    private readonly HttpClient _http;
    private readonly int _min, _max;

    public NumberGenerator(HttpClient http, int min, int max) => (_http, _min, _max) = (http, min, max);

    public async Task<(byte[] key, byte[] hmac, int value)> GenerateCommitmentAsync()
    {
        var value = int.Parse((await _http.GetStringAsync($"http://www.randomnumberapi.com/api/v1.0/random?min={_min}&max={_max}&count=1")).Trim('[', ']'));
        var (key, hmac) = GenerateProof(value);
        return (key, hmac, value);
    }
}