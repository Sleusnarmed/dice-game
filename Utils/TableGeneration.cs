using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables;

namespace DiceGame.Utils;

public static class TableGeneration
{
    private const int PageSize = 5;
    private const int VisibleCols = 5;

    public static void DisplayDiceTable(List<Die> dice, int pageSize = PageSize)
    {
        int total = dice.Count;
        int rowPage = 1, colPage = 1;

        while (true)
        {
            Console.Clear();

            int r0 = (rowPage - 1) * pageSize;
            int c0 = (colPage - 1) * VisibleCols;

            var rows = dice.Skip(r0).Take(pageSize).ToList();
            var cols = dice.Skip(c0).Take(VisibleCols).ToList();

            var headers = new[] { "User \\ Opponent" }
                .Concat(cols.Select(GetTruncatedLabel)).ToArray();

            var table = new ConsoleTable(headers);

            foreach (var rowDie in rows)
            {
                var row = new[] { GetTruncatedLabel(rowDie) }
                    .Concat(cols.Select(colDie =>
                        ProbCalc.CalculateWinProbability(rowDie, colDie)))
                    .ToArray();

                table.AddRow(row);
            }

            int rowPages = (total + pageSize - 1) / pageSize;
            int colPages = (total + VisibleCols - 1) / VisibleCols;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Rows {r0 + 1}-{r0 + rows.Count} of {total} | Cols {c0 + 1}-{c0 + cols.Count} of {total}");
            Console.WriteLine($"Page {rowPage}/{rowPages} (rows) | {colPage}/{colPages} (cols)");
            table.Write(Format.Alternative);
            Console.ResetColor();

            Console.WriteLine("\n[←→] Columns [↑↓] Rows [F]irst [L]ast [Q]uit");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.RightArrow: colPage = Math.Min(colPage + 1, colPages); break;
                case ConsoleKey.LeftArrow: colPage = Math.Max(colPage - 1, 1); break;
                case ConsoleKey.UpArrow: rowPage = Math.Max(rowPage - 1, 1); break;
                case ConsoleKey.DownArrow: rowPage = Math.Min(rowPage + 1, rowPages); break;
                case ConsoleKey.F: rowPage = colPage = 1; break;
                case ConsoleKey.L:
                    rowPage = rowPages;
                    colPage = colPages;
                    break;
                case ConsoleKey.Q: return;
            }
        }
    }

    private static string GetTruncatedLabel(Die die)
    {
        return string.Join(",", die.Faces);
    }
}