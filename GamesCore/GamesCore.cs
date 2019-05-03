using System;
using System.Collections.Generic;

namespace GamesCore
{
    public abstract class Game<TState, TMove, TPlayer, TSummary, TJudge, TAction>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TJudge : IJudge<TState, TMove, TPlayer, TSummary, TAction>, new()
        where TAction : IAction
    {
        private readonly TJudge judge = new TJudge();
        public IUserInterface<TState, TMove, TPlayer, TSummary, TAction> UserInterface { get; set; }

        public void Start(TPlayer[] players)
        {
            foreach (TPlayer player in players)
                player.UserInterface = UserInterface;

            TState state = judge.SetStartState(players);
            while (judge.IsNotEnd(state))
            {
                UserInterface?.ShowState(state);
                TPlayer player = judge.NextPlayer(state);
                List<TMove> allowedMoves = judge.GetAllowedMoves(state, player);
                if (allowedMoves.Count == 0)
                    continue;
                TMove nextMove;
                do
                    nextMove = player.NextMove(state, allowedMoves);
                while (!allowedMoves.Contains(nextMove));
                state = nextMove.StateEnd;
            }
            UserInterface?.ShowEndState(state);
        }
    }

    public interface IUserInterface<TState, TMove, TPlayer, TSummary, TAction>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TAction : IAction
    {
        void ShowEndState(TState state);
        void ShowState(TState state);
        TMove ShowSelectionMove(TState state, TPlayer player, List<TMove> allowedMoves);
    }

    public interface IState<TSummary> : ICloneable
        where TSummary : ISummary
    {
        TSummary Summary { get; }
    }

    public interface ISummary
    {
    }

    public interface IMove<TState, TSummary, TAction>
        where TState : IState<TSummary>
        where TSummary : ISummary
        where TAction : IAction
    {
        TState StateStart { get; set; }
        TState StateEnd { get; set; }
        List<TAction> Actions { get; set; }
    }

    public interface IAction
    {
        bool IsAllowed { get; }
        bool IsEnd { get; }
    }

    public interface IJudge<TState, TMove, TPlayer, TSummary, TAction>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TAction : IAction
    {
        TState SetStartState(TPlayer[] players);
        bool IsNotEnd(TState state);
        TPlayer NextPlayer(TState state);
        List<TMove> GetAllowedMoves(TState state, TPlayer player);
    }

    public interface IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TAction : IAction
    {
        IUserInterface<TState, TMove, TPlayer, TSummary, TAction> UserInterface { get; set; }

        TMove NextMove(TState state, List<TMove> allowedMoves);
    }
}