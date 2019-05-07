using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LearningAIPlayer
{
    public enum Score
    {
        NotEnd,
        Win,
        Defeat,
        Draw,
        OppMove
    }

    public abstract class AIPlayer<TState>
    {
        public const string PATH_AI_FILE = @"AITree.json";
        public static Node<TState> Tree { get; set; } = GetAITree();

        private static Node<TState> GetAITree()
        {
            if (File.Exists(PATH_AI_FILE))
                return JsonConvert.DeserializeObject<Node<TState>>(File.ReadAllText(PATH_AI_FILE));
            return new Node<TState>();
        }

        public Node<TState> Root { get; set; }

        public TState GetNextMove(TState start, TState[] stateMoves)
        {
            SetRootNode(start);
            if (Root.Children == null)
                Root.Children = stateMoves.Select(CreateNode).ToList();
            Dictionary<Node<TState>, int> stats = Root.Children.Where(a => stateMoves.Contains(a.State)).ToDictionary(a => a, a => GetStats(a));
            int theBest = stats.Values.Max();
            Root = stats.First(a => a.Value == theBest).Key;
            return stateMoves.Single(a => a.Equals(Root.State));
        }

        private int GetStats(Node<TState> node)
        {
            //if (node.Score !=Score.NotEnd)
            switch (node.Score)
            {
                case Score.NotEnd:
                    //Node<TState>[] nodes = GetMyNode(node).ToArray();
                    //int drowCont = nodes.Count(a => a.Score == Score.Draw);
                    //int endCont = nodes.Count(a => a.Score != Score.NotEnd && a.Score != Score.Defeat);
                    return node.Children?.Sum(a => GetStats(a)) ?? 0;
                case Score.Win:
                    return 1;
                case Score.Defeat:
                    return -1;
                case Score.Draw:
                    return 0;
                case Score.OppMove:
                    return node.Children?.Sum(a => GetStats(a)) ?? 0;
                    default:
                    return 0;
            }
            //Node<TState>[] nodes = GetMyNode(node).ToArray();
            ////node.Score = SumScore(node);
            //int defeatCont = nodes.Count(a => a.Score == Score.Defeat);
            //if (node.Score == Score.Defeat)
            //    defeatCont++;
            //if (node.Score == Score.Win)
            //    defeatCont--;
            //int drowCont = nodes.Count(a => a.Score == Score.Draw);
            //int endCont = nodes.Count(a => a.Score != Score.NotEnd && a.Score != Score.Defeat);
            //var aasdasd = string.Join("\r\n", nodes.Select(a => a.State.ToString()));
            //return endCont - defeatCont;
        }

        private Score SumScore(Node<TState> root)
        {
            if (root.Children != null)
            {
                foreach (Node<TState> child in root.Children)
                    child.Score = SumScore(child);
                if (root.Score != Score.OppMove && GetMyNode(root).All(a => a.Score == Score.Defeat))
                    return Score.Defeat;
            }
            return root.Score;
        }

        private IEnumerable<Node<TState>> GetMyNode(Node<TState> root)
        {
            if (root.Children != null)
                foreach (Node<TState> child in root.Children)
                {
                    if (child.Score != Score.OppMove)
                        yield return child;
                    if (child.Score != Score.Defeat)
                        foreach (Node<TState> node in GetMyNode(child))
                            yield return node;
                }
        }

        public abstract Node<TState> CreateNode(TState state);

        void SetRootNode(TState state)
        {
            if (Root == null)
            {
                if (Tree.Children == null)
                    Tree.Children = new List<Node<TState>>();
                Root = Tree.Children.FirstOrDefault(a => a.State.Equals(state));
                if (Root == null)
                {
                    Root = new Node<TState> { State = state };
                    Tree.Children.Add(Root);
                }
            }
            else
            {
                if (Root.Children == null)
                    Root.Children = new List<Node<TState>>();
                Node<TState> node = Root.Children.FirstOrDefault(a => a.State.Equals(state));
                if (node == null)
                {
                    node = CreateNode(state);
                    node.Score = Score.OppMove;
                    Root.Children.Add(node);
                }

                Root = node;
            }
        }
    }

    public class Node<TState>
    {
        public TState State { get; set; }
        public Score Score { get; set; }
        public List<Node<TState>> Children { get; set; }
    }
}