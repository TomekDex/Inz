using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamesCore
{
    public class Game<TState, TMove, TPlayer, TSummary, TJudge, TAction, TSettings>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TJudge : IJudge<TState, TMove, TPlayer, TSummary, TAction, TSettings>, new()
        where TAction : IAction
        where TSettings : ISettings
    {
        private readonly TJudge judge = new TJudge();
        public IUserInterface<TState, TMove, TPlayer, TSummary, TAction> UserInterface { get; set; }

        public async Task Start(TSettings settings, params TPlayer[] players)
        {
            foreach (TPlayer player in players)
                player.UserInterface = UserInterface;

            TState state = judge.SetStartState(settings, players);
            while (judge.IsNotEnd(state))
            {
                await UserInterface?.ShowState(state);
                TPlayer player = judge.NextPlayer(state);
                List<TMove> allowedMoves = judge.GetAllowedMoves(state, player);
                if (allowedMoves.Count == 0)
                    continue;
                TMove nextMove;
                do
                    nextMove = await player.NextMove(state, allowedMoves);
                while (!allowedMoves.Contains(nextMove));
                state = nextMove.StateEnd;
            }

            foreach (TPlayer player in players)
                player.AnalyzeResult(state);

            await UserInterface?.ShowEndState(state);
        }
    }

    public interface ISettings
    {
    }

    public interface IUserInterface<TState, TMove, TPlayer, TSummary, TAction>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TAction : IAction
    {
        Task ShowEndState(TState state);
        Task ShowState(TState state);
        Task<TMove> ShowSelectionMove(TState state, TPlayer player, List<TMove> allowedMoves);
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

    public interface IJudge<TState, TMove, TPlayer, TSummary, TAction, TSettings>
        where TState : IState<TSummary>
        where TMove : IMove<TState, TSummary, TAction>
        where TPlayer : IPlayer<TState, TMove, TPlayer, TSummary, TAction>
        where TSummary : ISummary
        where TAction : IAction
        where TSettings : ISettings
    {
        TState SetStartState(TSettings settings, TPlayer[] players);
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

        void AnalyzeResult(TState state);
        Task<TMove> NextMove(TState state, List<TMove> allowedMoves);
    }
}