using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame.Utils;

public readonly struct Die
{
    public int[] Faces { get; }
    public int FaceCount => Faces.Length;
    public Die(int[] faces) => Faces = faces;
    public int GetFaceValue(int index) => Faces[index];
}

public static class DiceParser
{
    public static List<Die> ParseDice(ReadOnlySpan<string> args)
    {
        if (args.Length < 3) throw new ArgumentException("At least 3 dice must be provided. Example: 1,2,3,4,5,6 7,8,9,10,11,12");

        var dice = new List<Die>(args.Length);
        int? expectedFaceCount = null;

        foreach (var arg in args)
        {
            var faces = ParseDieString(arg);
            expectedFaceCount ??= faces.Length;

            if (faces.Length != expectedFaceCount)
                throw new ArgumentException($"This is a fair dice game, so all dice must have the same number of faces. Expected {expectedFaceCount} faces, but got {faces.Length} in: {arg}");

            dice.Add(new Die(faces));
        }

        return dice;
    }

    private static int[] ParseDieString(string dieString)
    {
        return dieString.Split(',')
            .Select(s => int.TryParse(s.Trim(), out int val) && val >= 0
                ? val
                : throw new ArgumentException($"Invalid face value: '{s}'. Must be non-negative *integer* and can't be a letter"))
            .ToArray();
    }
}