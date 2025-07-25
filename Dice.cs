using System;
using System.Collections.Generic;

namespace DiceGame.Utils;

public readonly struct Die
{
    public int[] Faces { get; }
    public int FaceCount => Faces.Length;

    public Die(int[] faces)
    {
        Faces = faces;
    }

    public int GetFaceValue(int index) => Faces[index];
}

public static class DiceParser
{
    public static List<Die> ParseDice(ReadOnlySpan<string> args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("At least 2 dice must be provided. Example: 1,2,3,4,5,6 7,8,9,10,11,12");
        }

        var dice = new List<Die>(args.Length);
        // This is for setting the faces of the first die to match the next ones
        int expectedFaceCount = -1;

        foreach (var arg in args)
        {
            var faceValues = ParseDieString(arg);
            if (expectedFaceCount == -1)
            {
                expectedFaceCount = faceValues.Length;
            }
            else if (faceValues.Length != expectedFaceCount)
            {
                throw new ArgumentException($"This is a fair dice game, so all dice must have the same number of faces. Expected {expectedFaceCount} faces, but got {faceValues.Length} in: {arg}");
            }

            dice.Add(new Die(faceValues));
        }

        return dice;
    }

    private static int[] ParseDieString(string dieString)
    {
        var segments = dieString.Split(',');
        var result = new int[segments.Length];

        for (int i = 0; i < segments.Length; i++)
        {
            if (!int.TryParse(segments[i].AsSpan().Trim(), out var value))
            {
                throw new ArgumentException($"Invalid die format: '{dieString}'. Must be comma-separated integers (e.g., '1,2,3,4,5,6')");
            }
            if (value < 0)
            {
                throw new ArgumentException(
                    $"Negative face value: {value}\n" +
                    "All faces must be non-negative. Example: '0,1,2'");
            }

            result[i] = value;
        }

        return result;
    }
}