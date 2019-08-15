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

    public abstract class AIPlayer<TState>
    {
        public const string PATH_AI_FILE = @"AITree.json";

        public static Dictionary<TState, Node<TState>> Tree { get; set; } = GetAITree();

        public static Task saveTask;
        public static object saveSemafore = new object();
        public static object addNodeSemafore = new object();

        public TState Root { get; set; }

        private static Dictionary<TState, Node<TState>> GetAITree()
        {
            if (File.Exists(PATH_AI_FILE))
                return JsonConvert.DeserializeObject<KeyValuePair<TState, Node<TState>>[]>(File.ReadAllText(PATH_AI_FILE)).ToDictionary(a => a.Key, a => a.Value);
            return new Dictionary<TState, Node<TState>>();
        }

        private static void SaveTree()
        {
            Task newSaveTask = new Task(async () =>
                {
                    string tree;
                    lock (addNodeSemafore)
                        tree = JsonConvert.SerializeObject(Tree.ToArray());
                    File.WriteAllText(PATH_AI_FILE, tree);
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
            foreach (TState child in children)
            {
                if (!Tree[root].Children.Contains(child))
                {
                    lock (addNodeSemafore)
                        Tree[root].Children.Add(child);
                    Tree[root].Summary = Score.NotEnd;
                    if (!Tree.ContainsKey(child))
                        lock (addNodeSemafore)
                            Tree.Add(child, CreateNode(child));
                    anyNew = true;
                }
            }

            if (anyNew)
                SaveTree();
        }

        public TState GetNextMove(TState start, TState[] stateMoves)
        {
            SetRoot(start, stateMoves);
            Root = Tree[Root].GetNext();
            return stateMoves.Single(a => a.Equals(Root));
        }

        private void SetRoot(TState start, TState[] stateMoves)
        {
            if (!Tree.ContainsKey(start))
            {
                lock (addNodeSemafore)
                    Tree.Add(start, CreateNode(start));
                if (Root != null)
                    lock (addNodeSemafore)
                        Tree[Root].Children.Add(start);
            }

            AddNode(start, stateMoves);
            Root = start;
        }

        public abstract Node<TState> CreateNode(TState state);
    }

    public class Node<TState>
    {
        public bool End { get; set; }
        public bool Winner { get; set; }

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

                if (Children.Any(a => AIPlayer<TState>.Tree[a].Summary == Score.Win))
                    return summary = Score.Defeat;
                if (Children.Any(a => AIPlayer<TState>.Tree[a].Summary == Score.NotEnd))
                    return Score.NotEnd;
                if (Children.Any(a => AIPlayer<TState>.Tree[a].Summary == Score.Draw))
                    return summary = Score.Draw;
                if (Children.Any(a => AIPlayer<TState>.Tree[a].Summary == Score.Defeat))
                    return summary = Score.Win;
                return Score.NotEnd;
            }
            set
            {
                summary = value;
            }
        }
        private Score summary = Score.NotEnd;

        public List<TState> Children { get; set; } = new List<TState>();

        public TState GetNext()
        {
            TState next = Children.FirstOrDefault(a => AIPlayer<TState>.Tree[a].Summary == Score.Win);
            if (next == null)
                next = Children.FirstOrDefault(a => AIPlayer<TState>.Tree[a].Summary == Score.NotEnd);
            if (next == null)
                next = Children.FirstOrDefault(a => AIPlayer<TState>.Tree[a].Summary == Score.Draw);
            if (next == null)
                next = Children.First();
            return next;
        }
    }
}