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
            TableGeneration.DisplayDiceTable(dice);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}