using System.Collections.Generic;
using Common.Graph.Interfaces;

namespace Common.Graph
{
    public class Dijkstra: IShortestPathAlgorithm
    {
        void PopulateShortestPathGraph(IVertex startVertex, IVertex destinationVertex)
        {
            startVertex.MinDistance = 0;

            SortedSet<IVertex> vertices = new SortedSet<IVertex>();
            HashSet<IVertex> visitedVertices = new HashSet<IVertex>();

            vertices.Add(startVertex);

            while (vertices.Count > 0)
            {
                var u = vertices.Min;
                vertices.Remove(u);

                visitedVertices.Add(u);

                if (u == destinationVertex)
                    return;

                foreach (var edge in u.Adjacencies)
                {
                    var v = edge.DestinationVertex;
                    if(visitedVertices.Contains(v))
                        continue;

                    double distanceThroughU = u.MinDistance + CalculateCostToVertex(edge);
                    if (distanceThroughU < v.MinDistance)
                    {
                        vertices.Remove(v);
                        v.MinDistance = distanceThroughU;
                        v.previousVertex = u;
                        vertices.Add(v);
                    }
                }
            }            
        }

        public virtual double CalculateCostToVertex(IEdge edge)
        {
            var v = edge.DestinationVertex;
            return edge.Weight + v.Weight;
        }

        public IList<IVertex> GetShortestPath(IVertex startVertex, IVertex destinationVertex)
        {
            PopulateShortestPathGraph(startVertex, destinationVertex);

            List<IVertex> path = new List<IVertex>();

            for (var v = destinationVertex; v != null; v = v.previousVertex )
            {
                path.Add(v);
            }

            path.Reverse();

            return path;
        }
    }
}
