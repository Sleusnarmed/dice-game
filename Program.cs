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

                int guess = ShowMenu("Try to guess my selection:", new[] { "0 - 0", "1 - 1" });
                Console.WriteLine($"Your selection: {guess}\nMy selection: {firstMoveValue} \n(KEY={Convert.ToHexString(key)})");
                bool playerFirst = guess == firstMoveValue;

                var availableDice = new List<Die>(dice);
                var playerDie = playerFirst
                    ? await PlayerSelectsDie(availableDice)
                    : availableDice[0];
                availableDice.Remove(playerDie);
                var computerDie = playerFirst
                    ? availableDice[0]
                    : ComputerSelectsDie(availableDice);

                Console.WriteLine($"\n{(playerFirst ? "You" : "I")} make the first move.");

                int compRoll = await PerformRoll(game, diceRollGen, computerDie, "Computer");
                int playerRoll = await PerformRoll(game, diceRollGen, playerDie, "Player");

                Console.WriteLine($"\nComputer: {compRoll} (Die: [{string.Join(",", computerDie.Faces)}])\n" +
                                  $"Player: {playerRoll} (Die: [{string.Join(",", playerDie.Faces)}])\n" +
                                  (playerRoll > compRoll ? "You win!" :
                                   compRoll > playerRoll ? "Computer wins!" : "It's a tie!"));
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
        }

        static int ShowMenu(string prompt, string[] options)
        {
            while (true)
            {
                Console.WriteLine($"{prompt}\n{string.Join("\n", options)}\nX - exit\n? - help");
                Console.Write("Your selection: ");
                var input = Console.ReadLine()?.ToUpper();

                if (input == "X") Environment.Exit(0);
                if (input == "?") { DisplayHelp(); continue; }

                if (int.TryParse(input, out int choice) && choice >= 0 && choice < options.Length)
                    return choice;

                Console.WriteLine("Invalid input. Please try again.");
            }
        }

        static async Task<Die> PlayerSelectsDie(List<Die> dice) =>
            dice[ShowMenu("Choose your die:", dice.Select((d, i) => $"{i} - {string.Join(",", d.Faces)}").ToArray())];

        static Die ComputerSelectsDie(List<Die> dice)
        {
            var choice = dice[0];
            Console.WriteLine($"I choose [{string.Join(",", choice.Faces)}]");
            return choice;
        }

        static async Task<int> PerformRoll(DiceLogic game, INumberGenerator gen, Die die, string roller)
        {
            Console.WriteLine($"\n{roller} roll:");
            var (key, hmac, compChoice) = await gen.GenerateCommitmentAsync();
            Console.WriteLine($"HMAC: {Convert.ToHexString(hmac)}");

            int input = ShowMenu("Add your number:", Enumerable.Range(0, die.FaceCount).Select(i => $"{i} - {i}").ToArray());

            var (combinedIndex, result) = game.RollDie(die, input, compChoice);

            Console.WriteLine($"My number: {combinedIndex} (KEY={Convert.ToHexString(key)})");
            Console.WriteLine($"Fair result: ({combinedIndex} + {input}) % {die.FaceCount} = {(combinedIndex + input) % die.FaceCount}");
            Console.WriteLine($"Result: {result}");

            return result;
        }

        static void DisplayHelp()
        {
            Console.WriteLine("\nHELP:\n- Guess (0) or (1) to go first\n- Select your die\n- Add your number\n- Verify cryptographic fairness\nPress any key...");
            Console.ReadKey();
        }
    }
}
