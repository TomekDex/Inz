using System.Collections.Generic;
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
            IsAllowed = state.Border[x, y] == default;
        }

        public byte X { get; }
        public byte Y { get; }
        public TicTacToePlayerType PlayerType { get; }
        public bool IsAllowed { get; }
        public bool IsEnd { get; } = true;

        public void Execute(TicTacToeState state)
        {            
            state.Border[X, Y] = PlayerType;
        }
    }
}