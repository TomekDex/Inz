using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public static Node<TState, TPlayer> Tree { get; set; }
        public static Dictionary<TState, Node<TState, TPlayer>> DictionaryTree { get; set; }
        public static Task saveTask;
        public static object saveSemafore = new object();
        public static object addNodeSemafore = new object();

        public Node<TState, TPlayer> Root { get; set; }

        static AIPlayer()
        {
            Tree = GetAITree();
            DictionaryTree = new Dictionary<TState, Node<TState, TPlayer>>();
            if (Tree != null)
                foreach (Node<TState, TPlayer> node in Tree.GetNodes())
                    if (!DictionaryTree.ContainsKey(node.State))
                        DictionaryTree.Add(node.State, node);
        }

        private static Node<TState, TPlayer> GetAITree()
        {
            if (File.Exists(PATH_AI_FILE))
                return JsonConvert.DeserializeObject<Node<TState, TPlayer>>(File.ReadAllText(PATH_AI_FILE));
            return null;
        }

        private static void SaveTree()
        {
            Task newSaveTask = new Task(async () =>
                {
                    lock (addNodeSemafore)
                        File.WriteAllText(PATH_AI_FILE, JsonConvert.SerializeObject(Tree));
                    await Task.Delay(10000);
                    if (saveTask != null)
                    {
                        saveTask?.Start();
                        saveTask = null;
                    }
                });

            if (saveTask == null)
            {
                saveTask = newSaveTask;
                saveTask?.Start();
                saveTask = null;
            }
            else
                saveTask = newSaveTask;
        }

        public void AddNode(TState root, params TState[] children)
        {
            bool anyNew = false;
            Node<TState, TPlayer>[] tree = Tree.GetNodes().ToArray();
            foreach (TState child in children)
            {
                Node<TState, TPlayer> nodechild;
                if (!DictionaryTree.ContainsKey(child))
                    nodechild = CreateNode(child);
                else
                    nodechild = DictionaryTree[child];

                foreach (Node<TState, TPlayer> node in tree.Where(a => a.State.Equals(root)))
                    if (!node.Children.Contains(nodechild))
                    {
                        lock (addNodeSemafore)
                            node.Children.Add(nodechild);
                        node.Summary = Score.NotEnd;
                        if (!DictionaryTree.ContainsKey(nodechild.State))
                            DictionaryTree.Add(nodechild.State, nodechild);
                        anyNew = true;
                    }
            }

            if (anyNew)
                SaveTree();
        }

        public TState GetNextMove(TState start, TState[] stateMoves, TPlayer player)
        {
            SetRoot(start, stateMoves);
            Root = Root.GetNext();
            return stateMoves.Single(a => a.Equals(Root.State));
        }

        private void SetRoot(TState start, TState[] stateMoves)
        {
            if (!DictionaryTree.ContainsKey(start))
            {
                Node<TState, TPlayer> node = CreateNode(start);
                if (Tree == null)
                    Root = Tree = node;
                else
                    AddNode(Root.State, start);
            }

            AddNode(start, stateMoves);
            Root = DictionaryTree[start];
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
                if (summary != Score.NotEnd)
                    return summary;

                if (End)
                {
                    if (Winner)
                        return summary = Score.Win;
                    else
                        return summary = Score.Draw;
                }

                if (Children.Any(a => a.Summary == Score.Win))
                    return summary = Score.Defeat;
                if (Children.Any(a => a.Summary == Score.NotEnd))
                    return Score.NotEnd;
                if (Children.Any(a => a.Summary == Score.Draw))
                    return summary = Score.Draw;
                if (Children.Any(a => a.Summary == Score.Defeat))
                    return summary = Score.Win;
                return Score.NotEnd;
            }
            set
            {
                summary = value;
            }
        }
        private Score summary = Score.NotEnd;

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