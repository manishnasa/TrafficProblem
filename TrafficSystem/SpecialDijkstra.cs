using System;
using System.Collections.Generic;
using Common.Graph;
using Common.Graph.Interfaces;
using Common.Logger.Interfaces;
using Common.Logger;

namespace TrafficSystem
{
    public class SpecialDijkstra: Dijkstra
    {
        SpecialDijkstra() { }

        ILogger Logger = DumbConsoleLogger.GetInstance();

        static SpecialDijkstra Instance;
        public static SpecialDijkstra GetInstance()
        {
            if( Instance == null)
                Instance = new SpecialDijkstra();

            return Instance;
        }

        public override double CalculateCostToVertex(IEdge edge)
        {
            var start = edge.StartVertex;
            var destination = edge.DestinationVertex;

            Logger.Info("It will take " + edge.Weight + " seconds to traverse this street.");
            
            var totalCost = edge.Weight + destination.GetWeight(start);

            Logger.Info("\n+++ Total time of travel for this street = " + totalCost + " seconds. +++");

            return totalCost;            
        }
    }
}
