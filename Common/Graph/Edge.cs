using System;
using Common.Graph.Interfaces;

namespace Common.Graph
{
    public class Edge: IEdge
    {
        public IVertex StartVertex { get; set; }
        public IVertex DestinationVertex { get; set; }
        public double Weight { get; set; }

        public Edge(IVertex startVertex, IVertex destinationVertex)
        {
            if (startVertex == null || destinationVertex == null)
                throw new ArgumentNullException("Null arguments in Edge constructor!");

            StartVertex = startVertex;
            DestinationVertex = destinationVertex;
            StartVertex.Adjacencies.Add(this);
        }

        public Edge(IVertex startVertex, IVertex destinationVertex, double weight): this(startVertex, destinationVertex)
        {            
            Weight = weight;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            IEdge e = obj as IEdge;
            if (e == null) return false;

            return ( StartVertex.Equals(e.StartVertex) && DestinationVertex.Equals(e.DestinationVertex) );
        }

        public override int GetHashCode()
        {
            //TODO This weekens the hash function. Sum as hash is probably a bad idea. Because 6 = 2 + 4, 1 + 5 .
            //Need to come up with a better way to do it.
            return (StartVertex.GetHashCode() + DestinationVertex.GetHashCode());
        }
    }
}
