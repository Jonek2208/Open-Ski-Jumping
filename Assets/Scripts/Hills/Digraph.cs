using System.Collections.Generic;
using System.Linq;

namespace OpenSkiJumping.Hills
{
    public static class Digraph
    {
        public static List<string> TopologicalSort(IEnumerable<(string, string)> edgesList)
        {
            var graph = new Dictionary<string, List<string>>();
            var inDeg = new Dictionary<string, int>();

            foreach (var (from, to) in edgesList)
            {
                if (!graph.ContainsKey(from))
                {
                    graph.Add(from, new List<string>());
                    inDeg.Add(from, 0);
                }

                if (!graph.ContainsKey(to))
                {
                    graph.Add(to, new List<string>());
                    inDeg.Add(to, 0);
                }

                inDeg[to]++;
                graph[from].Add(to);
            }

            var bag = inDeg.Where(it => it.Value == 0).Select(it => it.Key).ToList();
            var result = new List<string>();

            while (bag.Count > 0)
            {
                var lastIndex = bag.Count - 1;
                var item = bag[lastIndex];
                bag.RemoveAt(lastIndex);
                result.Add(item);
                
                foreach (var v in graph[item])
                {
                    inDeg[v]--;
                    if (inDeg[v] == 0)
                    {
                        bag.Add(v);
                    }
                }
            }

            return result;
        }
    }
}