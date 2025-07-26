using System.Linq;

namespace DiceGame.Utils;

public static class ProbCalc
{
    public static string CalculateWinProbability(Die a, Die b) =>
        a.Faces.SequenceEqual(b.Faces) ? "0.5000" :
        a.Faces.Max() < b.Faces.Min() ? "0.0000" :
        (a.Faces.Sum(x => b.Faces.Count(y => x > y)) / (double)(a.FaceCount * b.FaceCount)).ToString("0.0000");
}