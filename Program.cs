using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using DiceGame.Utils;

namespace DiceGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using var http = new HttpClient();
                var dice = DiceParser.ParseDice(args);

                using var firstMoveGen = new NumberGenerator(http, 0, 50);
                using var diceRollGen = new NumberGenerator(http, 0, 5);
                var game = new DiceLogic();

                Console.WriteLine("Let's determine who makes the first move.");
                var (key, hmac, num) = await firstMoveGen.GenerateCommitmentAsync();
                int firstMoveValue = num % 2;
                Console.WriteLine($"HMAC: {Convert.ToHexString(hmac)}");

                int guess = ShowMenu("Try to guess my selection:", new[] { "0 - 0", "1 - 1" }, dice);
                Console.WriteLine($"Your selection: {guess}\nMy selection: {firstMoveValue} \n(KEY={Convert.ToHexString(key)})");
                bool playerFirst = guess == firstMoveValue;

                var availableDice = dice.ToList();
                Die playerDie, computerDie;

                if (playerFirst)
                {
                    playerDie = await PlayerSelectsDie(availableDice, dice);
                    availableDice.Remove(playerDie);
                    computerDie = availableDice[0];
                }
                else
                {
                    computerDie = ComputerSelectsDie(availableDice);
                    availableDice.Remove(computerDie);
                    playerDie = await PlayerSelectsDie(availableDice, dice);
                }

                Console.WriteLine("LET'S ROLL SOME DICES!");

                int compRoll = await PerformRoll(game, diceRollGen, computerDie, "Computer", dice);
                int playerRoll = await PerformRoll(game, diceRollGen, playerDie, "Player", dice);

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
                Console.Write("Your selection: ");
                var input = Console.ReadLine()?.ToUpper();

                if (input == "X") Environment.Exit(0);
                if (input == "H") { TableGeneration.DisplayDiceTable(dice); continue; }

                if (int.TryParse(input, out int choice) && choice >= 0 && choice < options.Length)
                    return choice;

                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        static async Task<Die> PlayerSelectsDie(List<Die> dice, List<Die> allDice) =>
            dice[ShowMenu("Choose your die:", dice.Select((d, i) => $"{i} - {string.Join(",", d.Faces)}").ToArray(), allDice)];

        static Die ComputerSelectsDie(List<Die> dice)
        {
            var choice = dice[0];
            Console.WriteLine($"I choose the die [{string.Join(",", choice.Faces)}]");
            return choice;
        }

        static async Task<int> PerformRoll(DiceLogic game, INumberGenerator gen, Die die, string roller, List<Die> dice)
        {
            Console.WriteLine($"\n{roller} roll:");
            var (key, hmac, compChoice) = await gen.GenerateCommitmentAsync();
            Console.WriteLine($"HMAC: {Convert.ToHexString(hmac)}");

            int input = ShowMenu("Add your number:", Enumerable.Range(0, die.FaceCount).Select(i => $"{i} - {i}").ToArray(), dice);

            var (combinedIndex, _) = game.RollDie(die, input, compChoice);
            int faceIndex = (combinedIndex + input) % die.FaceCount;
            int faceValue = die.Faces[faceIndex];

            Console.WriteLine($"My number: {combinedIndex} (KEY={Convert.ToHexString(key)})");
            Console.WriteLine($"Fair result: ({combinedIndex} + {input}) % {die.FaceCount} = {faceIndex}");
            Console.WriteLine($"Die face value: {faceValue}");

            return faceValue;
        }
    }
}