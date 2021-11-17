using System.Collections.Generic;
using System.Linq;

namespace OpenSkiJumping.Hills
{
    public static class Digraph
    {
        public static List<string> TopologicalSort(IEnumerable<string> vertices, IEnumerable<(string, string)> edges)
        {
            var verticesList = vertices.ToList();
            var graph = verticesList.ToDictionary(it => it, it => new List<string>());
            var inDeg = verticesList.ToDictionary(it => it, it => 0);

            foreach (var (from, to) in edges)
            {
                inDeg[to]++;
                graph[from].Add(to);
            }

            var bag = new Queue<string>(inDeg.Where(it => it.Value == 0).Select(it => it.Key));
            var result = new List<string>();

            while (bag.Count > 0)
            {
                var item = bag.Dequeue();
                result.Add(item);
                
                foreach (var v in graph[item])
                {
                    inDeg[v]--;
                    if (inDeg[v] == 0)
                    {
                        bag.Enqueue(v);
                    }
                }
            }

            return result;
        }
    }
}