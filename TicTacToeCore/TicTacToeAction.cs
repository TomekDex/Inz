using GamesCore;

namespace TicTacToeCore
{
    public class TicTacToeAction : IAction
    {
        public TicTacToeAction(byte x, byte y, TicTacToePlayerType playerType, TicTacToeState state)
        {
            X = x;
            Y = y;
            PlayerType = playerType;
            IsAllowed = state.Board[x, y] == default;
        }

        public byte X { get; }
        public byte Y { get; }
        public TicTacToePlayerType PlayerType { get; }
        public bool IsAllowed { get; }
        public bool IsEnd { get; } = true;

        public void Execute(TicTacToeState state)
        {
            state.Board[X, Y] = PlayerType;
        }
    }
}