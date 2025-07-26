using System;
using System.Collections.Generic;
namespace DiceGame.Utils;

public sealed class DiceLogic : IDisposable
{
    private readonly INumberGenerator _generator;
    private readonly List<Die> _dice;
    private Die? _computerDie, _playerDie;

    public DiceLogic(INumberGenerator generator, List<Die> dice)
    {
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        if (_dice.Count < 2) throw new ArgumentException("At least 2 dice required");
    }

    // Add these properties
    public Die ComputerDie => _computerDie ?? throw new InvalidOperationException("Computer die not assigned");
    public Die PlayerDie => _playerDie ?? throw new InvalidOperationException("Player die not assigned");

    // Rest of your existing methods...
    public async Task<(byte[] key, byte[] hmac, int computerChoice)> GetFirstMoveCommitmentAsync()
        => await _generator.GenerateCommitmentAsync();

    public void AssignDice(bool playerPicksFirst)
    {
        _playerDie = playerPicksFirst ? _dice[0] : _dice[1];
        _computerDie = playerPicksFirst ? _dice[1] : _dice[0];
    }

    public async Task<(byte[] key, byte[] hmac, int combinedIndex, int result)> RollDieAsync(Die die, int playerChoice)
    {
        if (playerChoice < 0 || playerChoice >= die.Faces.Length)
            throw new ArgumentOutOfRangeException(nameof(playerChoice));

        var (key, hmac, computerChoice) = await _generator.GenerateCommitmentAsync();
        int combinedIndex = (computerChoice + playerChoice) % die.Faces.Length;
        return (key, hmac, combinedIndex, die.GetFaceValue(combinedIndex));
    }

    public bool IsPlayerWinner(int playerRoll, int computerRoll) 
        => playerRoll > computerRoll;

    public void Dispose() => _generator.Dispose();
}