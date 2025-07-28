using System.Net.Http;
using System.Net.Http.Json;
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
        var numbers = await _http.GetFromJsonAsync<int[]>($"http://www.randomnumberapi.com/api/v1.0/random?min={_min}&max={_max}&count=1");
        var value = numbers?[0] ?? throw new FormatException("Invalid API response");
        var (key, hmac) = GenerateProof(value); // Uses HMAC-SHA3-256
        return (key, hmac, value);
    }
}