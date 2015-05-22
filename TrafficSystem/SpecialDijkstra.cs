using System;
using System.Collections.Generic;
using Common.Graph;
using Common.Graph.Interfaces;

namespace TrafficSystem
{
    public class SpecialDijkstra: Dijkstra
    {
        public override double CalculateCostToVertex(IEdge edge)
        {
            //TODO
            //Make singleton
            return 0.0;
        }
    }
}
