using System;
using DiceGame.Utils;

namespace DiceGame;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var dice = DiceParser.ParseDice(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}