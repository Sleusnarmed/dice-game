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
        if (args.Length < 3)
        {
            throw new ArgumentException("At least 3 dice must be provided. Example: 1,2,3,4,5,6 7,8,9,10,11,12");
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
        var faces = new int[segments.Length];

        for (int i = 0; i < segments.Length; i++)
        {
            if (!int.TryParse(segments[i].Trim(), out int val) || val < 0)
                throw new ArgumentException($"Invalid face value: '{segments[i]}'. Must be non-negative *integer* and can't be a letter");
            faces[i] = val;
        }
        return faces;
    }
}