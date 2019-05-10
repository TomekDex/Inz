using Newtonsoft.Json;
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
        Draw
    }

    public abstract class AIPlayer<TState, TPlayer>
    {
        public const string PATH_AI_FILE = @"AITree.json";
        public static Node<TState, TPlayer> Tree { get; set; } = GetAITree();

        private static Node<TState, TPlayer> GetAITree()
        {
            if (File.Exists(PATH_AI_FILE))
                return JsonConvert.DeserializeObject<Node<TState, TPlayer>>(File.ReadAllText(PATH_AI_FILE));
            return null;
        }

        public void AddNode(TState root, params TState[] children)
        {
            Node<TState, TPlayer>[] tree = Tree.GetNodes().ToArray();
            foreach (TState child in children)
            {
                Node<TState, TPlayer> nodechild = tree.AsParallel().FirstOrDefault(a => a.State.Equals(child));
                if (nodechild == null)
                    nodechild = CreateNode(child);
                foreach (Node<TState, TPlayer> node in tree.AsParallel().Where(a => a.State.Equals(root)))
                    if (!node.Children.Contains(nodechild))
                        node.Children.Add(nodechild);
            }
        }

        public Node<TState, TPlayer> Root { get; set; }

        public TState GetNextMove(TState start, TState[] stateMoves, TPlayer player)
        {
            SetRoot(start, stateMoves);
            Node<TState, TPlayer> next = Root.GetNext();
            Root = next;
            return stateMoves.Single(a => a.Equals(Root.State));
        }

        private void SetRoot(TState start, TState[] stateMoves)
        {
            Node<TState, TPlayer>[] nodes = Tree?.GetNodes()?.Where(a => a.State.Equals(start))?.ToArray();
            if ((nodes?.Length ?? 0) == 0)
            {
                Node<TState, TPlayer> node = CreateNode(start);
                if (Tree == null)
                    Root = Tree = node;
                else
                    AddNode(Root.State, start);
                Root = node;
            }
            else
                Root = nodes.First();

            AddNode(start, stateMoves);
        }

        public abstract Node<TState, TPlayer> CreateNode(TState state);
    }

    public class Node<TState, TPlayer>
    {
        public TState State { get; set; }
        public bool End { get; set; }
        public bool Winner { get; set; }
        public TPlayer Player { get; set; }
        public Score Summary
        {
            get
            {
                if (End)
                {
                    if (Winner)
                        return Score.Win;
                    else
                        return Score.Draw;
                }

                if (Children.Any(a => a.Summary == Score.Win))
                    return Score.Defeat;
                if (Children.Any(a => a.Summary == Score.NotEnd))
                    return Score.NotEnd;
                if (Children.Any(a => a.Summary == Score.Draw))
                    return Score.Draw;
                if (Children.Any(a => a.Summary == Score.Defeat))
                    return Score.Win;
                return Score.NotEnd;
            }
        }

        public List<Node<TState, TPlayer>> Children { get; set; } = new List<Node<TState, TPlayer>>();

        public Node<TState, TPlayer> GetNext()
        {
            Node<TState, TPlayer> next = Children.FirstOrDefault(a => a.Summary == Score.Win);
            if (next == null)
                next = Children.FirstOrDefault(a => a.Summary == Score.NotEnd);
            if (next == null)
                next = Children.FirstOrDefault(a => a.Summary == Score.Draw);
            if (next == null)
                next = Children.First();
            return next;
        }

        public IEnumerable<Node<TState, TPlayer>> GetNodes()
        {
            yield return this;
            if (Children?.Count > 0)
                foreach (Node<TState, TPlayer> child in Children)
                    foreach (Node<TState, TPlayer> node in child.GetNodes())
                        yield return node;
        }

        public override bool Equals(object obj)
        {
            return obj is Node<TState, TPlayer> node &&
                   EqualityComparer<TState>.Default.Equals(State, node.State);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TState>.Default.GetHashCode(State);
        }
    }
}