using GamesCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DraughtsCore
{
    public class DraughtsJudge : IJudge<DraughtsState, DraughtsMove, DraughtsPlayer, DraughtsSummary, DraughtsAction>
    {
        public List<DraughtsMove> GetAllowedMoves(DraughtsState state, DraughtsPlayer player)
        {
            List<DraughtsMove> moves = new List<DraughtsMove>();
            foreach (DraughtsActionHit hit in GetHit(player.PlayerType, null, state))
                foreach (List<DraughtsAction> hits in GetAllHits(player.PlayerType, state, new List<DraughtsAction> { hit }))
                    moves.Add(new DraughtsMove(state, hits));

            if (moves.Count == 0)
                foreach (KeyValuePair<Point, Vector[]> placeAndNeighbors in state.Border.PlacesAndNeighbors)
                    foreach (Vector neighbor in placeAndNeighbors.Value)
                    {
                        DraughtsActionMove actionMove = new DraughtsActionMove(player.PlayerType, placeAndNeighbors.Key, neighbor, state);
                        if (actionMove.IsAllowed)
                            moves.Add(new DraughtsMove(state, new List<DraughtsAction> { actionMove }));
                    }

            player.NoMove = moves.Count == 0;

            return moves;
        }

        private IEnumerable<List<DraughtsAction>> GetAllHits(DraughtsPlayerType playerType, DraughtsState state, List<DraughtsAction> hits)
        {
            DraughtsState stateTmp = (DraughtsState)state.Clone();
            foreach (DraughtsActionHit hit in hits)
                hit.Execute(stateTmp);
            IEnumerable<DraughtsActionHit> nextLevelHit = GetHit(playerType, (DraughtsActionHit)hits.Last(), stateTmp);
            if (!nextLevelHit.Any())
                yield return hits;
            else
                foreach (DraughtsActionHit hit in nextLevelHit)
                {
                    List<DraughtsAction> newHits = new List<DraughtsAction>(hits);
                    newHits.Add(hit);
                    foreach (List<DraughtsAction> newHit in GetAllHits(playerType, state, newHits))
                        yield return newHit;
                }
        }

        private static IEnumerable<DraughtsActionHit> GetHit(DraughtsPlayerType playerType, DraughtsActionHit hit, DraughtsState state)
        {
            foreach (KeyValuePair<Point, Vector[]> placeAndNeighbors in state.Border.PlacesAndNeighbors)
                if (hit == null || hit.PointTarget == placeAndNeighbors.Key)
                    foreach (Vector neighbor in placeAndNeighbors.Value)
                    {
                        DraughtsActionHit hitNew = new DraughtsActionHit(playerType, placeAndNeighbors.Key, neighbor, state, hit == null);
                        if (hit.IsAllowed)
                            yield return hitNew;
                    }
        }

        public bool IsNotEnd(DraughtsState state)
        {
            return !state.Players.Any(a=>a.NoMove);
        }

        public DraughtsPlayer NextPlayer(DraughtsState state)
        {
            state.CurentPlayer = state.CurentPlayer == DraughtsPlayerType.Black ? DraughtsPlayerType.White : DraughtsPlayerType.White;
            return state.Players.First(a => a.PlayerType == state.CurentPlayer);
        }

        public DraughtsState SetStartState(DraughtsPlayer[] players)
        {
            if (players.Length != 2)
                throw new Exception("Bad number of players");

            return new DraughtsState(players);
        }
    }
}