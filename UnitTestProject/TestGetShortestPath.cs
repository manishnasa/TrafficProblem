using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TrafficSystemContract;
using TrafficSystem;

namespace UnitTestProject
{
    [TestClass]
    public class TestGetShortestPath
    {
        [TestMethod]
        public void InterviewQuestion()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
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

            controlRoom.PairStreets("A", "X", "B");
            controlRoom.PairStreets("E", "X", "C");

            Dictionary<string, int> trafficAtX = new Dictionary<string, int>()
            {
                { "A", 3 },
                { "E", 3 }
            };

            Dictionary<string, Dictionary<string, int>> signalTraffic = new Dictionary<string, Dictionary<string, int>>()
            {
                { "X", trafficAtX }
            };

            IRoute route = controlRoom.GetFastestRoute("E", "B", signalTraffic);
            Assert.AreEqual(route.Path.Count, 2);
            Assert.AreEqual(route.Path[0].Name, "E");            
            Assert.AreEqual(route.Path[1].Name, "B");
        }

        [TestMethod]
        public void TestExpensiveEToB()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
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
            controlRoom.AddDoubleStreet("B", "E", 10, speed);
            controlRoom.AddDoubleStreet("B", "X", 1, speed);
            controlRoom.AddDoubleStreet("C", "D", 2, speed);
            controlRoom.AddDoubleStreet("C", "X", 0.5, speed);
            controlRoom.AddDoubleStreet("E", "X", 0.5, speed);

            controlRoom.PairStreets("A", "X", "B");
            controlRoom.PairStreets("E", "X", "C");

            Dictionary<string, int> trafficAtX = new Dictionary<string, int>()
            {
                { "A", 3 },
                { "E", 3 }
            };

            Dictionary<string, Dictionary<string, int>> signalTraffic = new Dictionary<string, Dictionary<string, int>>()
            {
                { "X", trafficAtX }
            };

            IRoute route = controlRoom.GetFastestRoute("E", "B", signalTraffic);
            Assert.AreEqual(route.Path.Count, 3);
            Assert.AreEqual(route.Path[0].Name, "E");
            Assert.AreEqual(route.Path[1].Name, "X");
            Assert.AreEqual(route.Path[2].Name, "B");
        }

        [TestMethod]
        public void TestTrafficAtTwoSignals()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            controlRoom.AddSignal("A");
            controlRoom.AddSignal("B", 30, 10);
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

            controlRoom.PairStreets("A", "X", "B");
            controlRoom.PairStreets("E", "X", "C");

            controlRoom.PairStreets("E", "B", "D");
            controlRoom.PairStreets("X", "B", "C");

            Dictionary<string, int> trafficAtX = new Dictionary<string, int>()
            {
                { "A", 3 },
                { "E", 3 }
            };

            Dictionary<string, int> trafficAtB = new Dictionary<string, int>()
            {
                { "E", 5 },
                { "X", 0 },
                { "C", 3 },
                { "D", 3 }
            };

            Dictionary<string, Dictionary<string, int>> signalTraffic = new Dictionary<string, Dictionary<string, int>>()
            {
                { "X", trafficAtX },
                { "B", trafficAtB }
            };

            IRoute route = controlRoom.GetFastestRoute("E", "B", signalTraffic);
            Assert.AreEqual(route.Path.Count, 2);
            Assert.AreEqual(route.Path[0].Name, "E");            
            Assert.AreEqual(route.Path[1].Name, "B");
        }

        [TestMethod]
        public void TestNoTrafficAtX()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            controlRoom.AddSignal("A");
            controlRoom.AddSignal("B", 30, 10);
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
            controlRoom.AddDoubleStreet("B", "X", 0.25, speed); //reduced distance here
            controlRoom.AddDoubleStreet("C", "D", 2, speed);
            controlRoom.AddDoubleStreet("C", "X", 0.5, speed);
            controlRoom.AddDoubleStreet("E", "X", 0.5, speed);

            controlRoom.PairStreets("A", "X", "B");
            controlRoom.PairStreets("E", "X", "C");

            controlRoom.PairStreets("E", "B", "D");
            controlRoom.PairStreets("X", "B", "C");

            IRoute route = controlRoom.GetFastestRoute("E", "B", null);
            Assert.AreEqual(route.Path.Count, 3);
            Assert.AreEqual(route.Path[0].Name, "E");
            Assert.AreEqual(route.Path[1].Name, "X");
            Assert.AreEqual(route.Path[2].Name, "B");
        }
    }
}
