using System;
using System.Collections.Generic;
using Common.Graph.Interfaces;

namespace Common.Graph
{
    public class Vertex: IVertex, IComparable
    {
        public string Name { get; set; }
        public IList<IEdge> Adjacencies { get; set; }

        public double Weight { get; set; }        
        public double MinDistance { get; set; }
        public IVertex previousVertex { get; set; }

        public Vertex(string name)
        {
             Name = name;
             Adjacencies = new List<IEdge>();

             Weight = 0.0;
             MinDistance = double.PositiveInfinity;
             previousVertex = null;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj is Vertex:CompareTo(obj) is null!");

            IVertex vertexToCompare = obj as IVertex;
            if( vertexToCompare == null )
                throw new ArgumentException("obj is Vertex:CompareTo(obj) is not a IVertex!");

            return MinDistance.CompareTo(vertexToCompare.MinDistance);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            
            IVertex v = obj as IVertex;
            if (v == null) return false;

            return Name.Equals(v.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }        
    }
}
