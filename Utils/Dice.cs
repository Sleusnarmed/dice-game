using System.Linq;

namespace DiceGame.Utils;

public readonly struct Die
{
    public int[] Faces { get; }
    public int FaceCount => Faces.Length;
    public Die(int[] faces) => Faces = faces;
}

public static class DiceParser
{
    public static List<Die> ParseDice(string[] args)
    {
        if (args.Length < 3) 
            throw new ArgumentException("At least 3 dice must be provided");

        var dice = args.Select(ParseDie).ToList();
        var faceCount = dice[0].FaceCount;

        if (dice.Any(d => d.FaceCount != faceCount))
            throw new ArgumentException($"All dice must have {faceCount} faces");

        return dice;
    }

    private static Die ParseDie(string dieString)
    {
        var faces = dieString.Split(',')
            .Select(s => int.TryParse(s.Trim(), out int val) && val >= 0
                ? val
                : throw new ArgumentException($"Invalid face value: '{s}'"))
            .ToArray();

        return new Die(faces);
    }
}