using System;
using System.Linq;
using System.Collections.Generic;
using DiceGame.Utils;

namespace DiceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dice = DiceParser.ParseDice(args);
                using var firstMoveGen = new SecureIntGen(0, 2);
                using var diceRollGen = new SecureIntGen(0, 6);
                var game = new DiceLogic();

                Console.WriteLine("Let's determine who makes the first move.");
                var (key, hmac, num) = firstMoveGen.GenerateCommitment();

                Console.WriteLine($"HMAC: {Convert.ToHexString(hmac)}");

                int guess = ShowMenu("Try to guess my selection:", new[] { "0 - 0", "1 - 1" }, dice);
                Console.WriteLine($"Your selection: {guess}\nMy selection: {num} \n(KEY={Convert.ToHexString(key)})");
                bool playerFirst = guess == num;

                var availableDice = dice.ToList();
                Die playerDie = playerFirst 
                    ? PlayerSelectsDie(availableDice, dice) 
                    : availableDice[0];
                Die computerDie = playerFirst 
                    ? availableDice[0] 
                    : PlayerSelectsDie(availableDice, dice);

                Console.WriteLine($"\nLET'S ROLL SOME DICE!\nI choose [{string.Join(",", computerDie.Faces)}]");

                int compRoll = PerformRoll(game, diceRollGen, computerDie, "Computer", dice);
                int playerRoll = PerformRoll(game, diceRollGen, playerDie, "Player", dice);
                
                Console.WriteLine($"\nComputer: {compRoll} (Die: [{string.Join(",", computerDie.Faces)}])\n" +
                                $"Player: {playerRoll} (Die: [{string.Join(",", playerDie.Faces)}])\n" +
                                (playerRoll > compRoll ? "You win!" :
                                compRoll > playerRoll ? "Computer wins!" : "It's a tie!"));
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static int ShowMenu(string prompt, string[] options, List<Die> dice)
        {
            while (true)
            {
                Console.WriteLine($"{prompt}\n{string.Join("\n", options)}\nX - exit\nH - help");
                var input = Console.ReadLine()?.ToUpper();
                if (input == "X") Environment.Exit(0);
                if (input == "H") { TableGeneration.DisplayDiceTable(dice); continue; }
                if (int.TryParse(input, out int choice) && choice >= 0 && choice < options.Length)
                    return choice;
                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        static Die PlayerSelectsDie(List<Die> dice, List<Die> allDice) =>
            dice[ShowMenu("Choose your die:", dice.Select((d, i) => $"{i} - {string.Join(",", d.Faces)}").ToArray(), allDice)];

        static int PerformRoll(DiceLogic game, SecureIntGen gen, Die die, string roller, List<Die> dice)
        {
            Console.WriteLine($"\n{roller} roll:");
            var (key, hmac, compChoice) = gen.GenerateCommitment();
            Console.WriteLine($"HMAC: {Convert.ToHexString(hmac)}");

            int input = ShowMenu("Add your number:", Enumerable.Range(0, die.FaceCount).Select(i => $"{i} - {i}").ToArray(), dice);
            var (combinedIndex, _) = game.RollDie(die, input, compChoice);
            int faceValue = die.Faces[(combinedIndex + input) % die.FaceCount];

            Console.WriteLine($"My number: {combinedIndex} (KEY={Convert.ToHexString(key)})\n" +
                            $"Fair result: ({combinedIndex} + {input}) % {die.FaceCount} = {(combinedIndex + input) % die.FaceCount}\n" +
                            $"Die face value: {faceValue}");

            return faceValue;
        }
    }
}