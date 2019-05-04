using DraughtsCore;
using GamesCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DraughtsConsole
{
    class DraughtsUI : IUserInterface<DraughtsState, DraughtsMove, DraughtsPlayer, DraughtsSummary, DraughtsAction>
    {
        public void ShowEndState(DraughtsState state)
        {
            ShowState(state);
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Winner: " + ChoosePlayerCharacter(state.Summary.Winner));
            Console.ReadKey();
        }

        public DraughtsMove ShowSelectionMove(DraughtsState state, DraughtsPlayer player, List<DraughtsMove> allowedMoves)
        {
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Choose move plater: " + ChoosePlayerCharacter(player.PlayerType));
            for (int i = 0; i < allowedMoves.Count; i++)
            {
                Console.WriteLine($"{i}:");
                Console.WriteLine(MoveToText(allowedMoves[i]));
            }
            int move;
            while (!int.TryParse(Console.ReadLine(), out move) || move < 0 || move >= allowedMoves.Count)
                ;
            return allowedMoves[move];
        }

        private string MoveToText(DraughtsMove draughtsMove)
        {
            string text = "";
            foreach (DraughtsAction action in draughtsMove.Actions)
            {
                if (action is DraughtsActionHit)
                {
                    DraughtsActionHit hit = action as DraughtsActionHit;
                    text += $"{hit.Point} --> Hit:{hit.PointTargetToHit} --> {hit.PointTarget}\r\n";
                }
                else
                    return $"{action.Point} --> {((DraughtsActionMove)action).PointTarget}\r\n";
            }
            return text.Trim();
        }

        public void ShowState(DraughtsState state)
        {
            Console.Clear();
            foreach (Point place in state.Board.PlacesAndNeighbors.Keys)
            {
                Console.SetCursorPosition(place.X, place.Y);
                Console.Write(ChoosePlayerCharacter(state.Board[place]));
            }
        }

        private char ChoosePlayerCharacter(DraughtsPlayerType? player)
        {
            switch (player)
            {
                case DraughtsPlayerType.White:
                    return 'W';
                case DraughtsPlayerType.Black:
                    return 'B';
                default:
                    return ' ';
            }
        }
    }
}