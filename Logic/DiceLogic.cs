namespace DiceGame.Utils;

public sealed class DiceLogic
{
    public (int CombinedIndex, int Result) RollDie(Die die, int playerChoice, int computerChoice)
    {
        if (playerChoice < 0 || playerChoice >= die.FaceCount)
            throw new ArgumentOutOfRangeException(nameof(playerChoice));

        int index = (computerChoice + playerChoice) % die.FaceCount;
        return (index, die.Faces[index]);
    }
}
