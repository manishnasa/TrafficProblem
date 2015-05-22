using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Common.Graph.Interfaces;
using Common.Graph;
using Common.Logger;
using TrafficSystemContract;
using TrafficSystem;

namespace UnitTestProject
{
    [TestClass]
    public class TestTrafficSystem
    {
        [TestMethod] 
        public void InterviewQuestion()
        {
            IShortestPathAlgorithm shortestPathAlgo = new Dijkstra();
            ITrafficControlRoom controlRoom = new TrafficControlRoom(shortestPathAlgo, DumbConsoleLogger.GetInstance());
            controlRoom.AddSignal("A");
            controlRoom.AddSignal("B");
            controlRoom.AddSignal("C");
            controlRoom.AddSignal("D");
            controlRoom.AddSignal("E");
            controlRoom.AddSignal("X", 30, 10);

            const int speed = 60;
            controlRoom.AddDoubleStreet("A", "D", 6, speed);
            controlRoom.AddDoubleStreet("A", "X", 1, speed);
            controlRoom.AddDoubleStreet("B", "C", 2, speed);
            controlRoom.AddDoubleStreet("B", "D", 2, speed);
            controlRoom.AddDoubleStreet("B", "E", 1, speed);
            controlRoom.AddDoubleStreet("B", "X", 1, speed);
            controlRoom.AddDoubleStreet("C", "D", 2, speed);
            controlRoom.AddDoubleStreet("C", "X", 0.5, speed);
            controlRoom.AddDoubleStreet("E", "X", 0.5, speed);
            
            Dictionary<string, int> signalTraffic = new Dictionary<string, int>()
            {
                { "X", 6 }
            };

            IRoute route = controlRoom.GetFastestRoute("E", "B", signalTraffic);
            Assert.AreEqual(route.Path.Count, 2);
            Assert.AreEqual(route.Path[0].Name, "E");
            Assert.AreEqual(route.Path[1].Name, "B");
        }

        [TestMethod] 
        public void InterviewQuestion_ExpensiveE2B()
        {
            IShortestPathAlgorithm shortestPathAlgo = new Dijkstra();
            ITrafficControlRoom controlRoom = new TrafficControlRoom(shortestPathAlgo, DumbConsoleLogger.GetInstance());
            controlRoom.AddSignal("A");
            controlRoom.AddSignal("B");
            controlRoom.AddSignal("C");
            controlRoom.AddSignal("D");
            controlRoom.AddSignal("E");
            controlRoom.AddSignal("X", 30, 10);

            const int speed = 60;
            controlRoom.AddDoubleStreet("A", "D", 6, speed);
            controlRoom.AddDoubleStreet("A", "X", 1, speed);
            controlRoom.AddDoubleStreet("B", "C", 2, speed);
            controlRoom.AddDoubleStreet("B", "D", 2, speed);
            controlRoom.AddDoubleStreet("B", "E", 1000, speed);
            controlRoom.AddDoubleStreet("B", "X", 1, speed);
            controlRoom.AddDoubleStreet("C", "D", 2, speed);
            controlRoom.AddDoubleStreet("C", "X", 0.5, speed);
            controlRoom.AddDoubleStreet("E", "X", 0.5, speed);
            
            Dictionary<string, int> signalTraffic = new Dictionary<string, int>()
            {
                { "X", 6 }
            };

            IRoute route = controlRoom.GetFastestRoute("E", "B", signalTraffic);
            Assert.AreEqual(route.Path.Count, 3);
            Assert.AreEqual(route.Path[0].Name, "E");
            Assert.AreEqual(route.Path[1].Name, "X");
            Assert.AreEqual(route.Path[2].Name, "B");
        }
    }
}

