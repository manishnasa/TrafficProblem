using System;
using System.Collections.Generic;

namespace Common.Graph.Interfaces
{
    public interface IVertex: IComparable
    {
        //Fixed properties
        string Name { get; set; }
        IList<IEdge> Adjacencies { get; set; }

        //Properties used by the shortest path algorithm
        double Weight { get; set; }
        double MinDistance { get; set; }
        IVertex previousVertex { get; set; }
    }
}
