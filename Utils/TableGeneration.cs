using System;
using System.Collections.Generic;
using ConsoleTables;

namespace DiceGame.Utils;

public static class TableGeneration
{
    private const int DefaultPageSize = 5;
    private const int MaxColumnWidth = 15;
    private const int MaxVisibleColumns = 8;
    private const string DefaultProbability = "0.3333"; //Temporary - to be replaced

    public static void DisplayDiceTable(List<Die> dice, int pageSize = DefaultPageSize)
    {
        int totalDice = dice.Count;
        int totalRowPages = (totalDice + pageSize - 1) / pageSize; 
        int totalColPages = (totalDice + MaxVisibleColumns - 1) / MaxVisibleColumns;
        int currentRowPage = 1, currentColPage = 1;

        while (true)
        {
            Console.Clear();

            int rowStart = (currentRowPage - 1) * pageSize;
            int rowEnd = Math.Min(rowStart + pageSize, totalDice);
            int colStart = (currentColPage - 1) * MaxVisibleColumns;
            int colEnd = Math.Min(colStart + MaxVisibleColumns, totalDice);
            var headers = new string[colEnd - colStart + 1];
            headers[0] = "User \\ Opponent";
            
            for (int i = 1; i < headers.Length; i++)
                headers[i] = GetTruncatedLabel(dice[colStart + i - 1]);

            var table = new ConsoleTable(headers);


            for (int row = rowStart; row < rowEnd; row++)
            {
                var rowData = new string[headers.Length];
                rowData[0] = GetTruncatedLabel(dice[row]);
                Array.Fill(rowData, DefaultProbability, 1, headers.Length - 1);
                table.AddRow(rowData);
            }

            // Display
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Rows {rowStart + 1}-{rowEnd} of {totalDice} | " +
                            $"Cols {colStart + 1}-{colEnd} of {totalDice}");
            Console.WriteLine($"Page {currentRowPage}/{totalRowPages} (rows) | " +
                            $"{currentColPage}/{totalColPages} (cols)");
            table.Write(Format.Alternative);
            Console.ResetColor();

            // Navigation
            Console.WriteLine("\n[←→] Columns [↑↓] Rows [F]irst [L]ast [Q]uit");
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow: currentColPage = Math.Min(currentColPage + 1, totalColPages); break;
                case ConsoleKey.LeftArrow: currentColPage = Math.Max(currentColPage - 1, 1); break;
                case ConsoleKey.UpArrow: currentRowPage = Math.Max(currentRowPage - 1, 1); break;
                case ConsoleKey.DownArrow: currentRowPage = Math.Min(currentRowPage + 1, totalRowPages); break;
                case ConsoleKey.F: currentRowPage = currentColPage = 1; break;
                case ConsoleKey.L: currentRowPage = totalRowPages; currentColPage = totalColPages; break;
                case ConsoleKey.Q: return;
            }
        }
    }

    private static string GetTruncatedLabel(Die die)
    {
        string label = string.Join(",", die.Faces);
        return label.Length <= MaxColumnWidth ? label : $"{label.AsSpan(0, MaxColumnWidth - 3)}...";
    }
}