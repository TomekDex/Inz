using System.Collections.Generic;

namespace TicTacToeCore
{
    public class TicTacToeBoard
    {
        private static readonly int[] endMasksX = GetWinMaskFor(TicTacToePlayerType.X);
        private static readonly int[] endMasksO = GetWinMaskFor(TicTacToePlayerType.O);
        private static readonly int fullMask = GetFullMask();
        private const int PLACE_SIZE = 2;
        private const int ROW_NUMBER = 3;
        private const int PLACE_MASK = 3;

        private static int GetFullMask()
        {
            TicTacToeBoard boardX = new TicTacToeBoard();
            TicTacToeBoard boardO = new TicTacToeBoard();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    boardX[i, j] = TicTacToePlayerType.X;
                    boardO[i, j] = TicTacToePlayerType.O;
                }

            return boardX.Board & boardO.Board;
        }

        private static int[] GetWinMaskFor(TicTacToePlayerType type)
        {
            List<int> mask = new List<int>();

            TicTacToeBoard board;
            for (int i = 0; i < 3; i++)
            {
                board = new TicTacToeBoard();
                board[i, 0] = type;
                board[i, 1] = type;
                board[i, 2] = type;
                mask.Add(board.Board);
                board = new TicTacToeBoard();
                board[0, i] = type;
                board[1, i] = type;
                board[2, i] = type;
                mask.Add(board.Board);
            }
            board = new TicTacToeBoard();
            board[0, 0] = type;
            board[1, 1] = type;
            board[2, 2] = type;
            mask.Add(board.Board);
            board = new TicTacToeBoard();
            board[0, 2] = type;
            board[1, 1] = type;
            board[2, 0] = type;
            mask.Add(board.Board);

            return mask.ToArray();
        }

        public int Board { get; set; }

        public TicTacToePlayerType this[int x, int y]
        {
            get
            {
                return (TicTacToePlayerType)((Board >> ((y * ROW_NUMBER + x) * PLACE_SIZE)) & PLACE_MASK);
            }

            set
            {
                Board |= (int)value << ((y * ROW_NUMBER + x) * PLACE_SIZE);
            }
        }

        public TicTacToePlayerType? GetScore()
        {
            for (int i = 0; i < endMasksX.Length; i++)
            {
                int result = endMasksO[i] & Board;
                if (result == endMasksX[i])
                    return TicTacToePlayerType.X;
                if (result == endMasksO[i])
                    return TicTacToePlayerType.O;
            }
            if ((fullMask & Board) == fullMask)
                return TicTacToePlayerType.No;
            return null;
        }
    }
}