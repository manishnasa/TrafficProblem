using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Common.Graph.Interfaces;
using Common.Graph;
using Common.Logger;
using TrafficSystemContract;
using TrafficSystem;
using Wintellect.PowerCollections;

namespace UnitTestProject
{
    [TestClass]
    public class TestTrafficControlRoom
    {
        [TestMethod]
        public void TestAddSignal()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");

            Assert.IsNotNull(signal);
            Assert.IsTrue(controlRoom.Signals.Contains(signal));
        }

        [TestMethod]
        public void TestAddDuplicateSignal()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);            

            ISignal signal2 = controlRoom.AddSignal("A");
            Assert.IsNull(signal2);
        }

        [TestMethod]
        public void TestAddSignalWithWaitTimes()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A", 30, 10);

            Assert.IsNotNull(signal);
            Assert.IsTrue(controlRoom.Signals.Contains(signal));
            Assert.AreEqual(signal.WaitTime.Seconds, 30);
            Assert.AreEqual(signal.PassthroughTimePerCar.Seconds, 10);
        }

        [TestMethod]
        public void TestAddSignalWithBadWaitTime()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A", -30, 10);

            Assert.IsNull(signal);
        }

        [TestMethod]
        public void TestAddStreet()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            ISignal signal2 = controlRoom.AddSignal("B");
            Assert.IsNotNull(signal2);

            IStreet street = controlRoom.AddStreet("A", "B", 10, 60);
            Assert.IsNotNull(street);

            IEdge edge = new Edge(signal, signal2);
            signal.Adjacencies.Contains(edge);
        }

        [TestMethod]
        public void TestAddStreetOnSameNode()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);
            
            IStreet street = controlRoom.AddStreet("A", "A", 10, 60);
            Assert.IsNull(street);            
        }

        [TestMethod]
        public void TestAddStreetWithBadDistance()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            ISignal signal2 = controlRoom.AddSignal("B");
            Assert.IsNotNull(signal2);

            IStreet street = controlRoom.AddStreet("A", "B", -10, 60);
            Assert.IsNull(street);
        }

        [TestMethod]
        public void TestAddDoubleStreet()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            ISignal signal2 = controlRoom.AddSignal("B");
            Assert.IsNotNull(signal2);

            Assert.IsTrue(controlRoom.AddDoubleStreet("A", "B", 10, 60));

            IEdge edge = new Edge(signal, signal2);
            signal.Adjacencies.Contains(edge);

            IEdge edge2 = new Edge(signal2, signal);            
            signal2.Adjacencies.Contains(edge2);
        }

        [TestMethod] 
        public void TestPairStreets()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            ISignal signal2 = controlRoom.AddSignal("B");
            Assert.IsNotNull(signal2);

            ISignal signal3 = controlRoom.AddSignal("C");
            Assert.IsNotNull(signal3);

            IStreet street = controlRoom.AddStreet("A", "B", 10, 60);
            Assert.IsNotNull(street);

            IStreet street2 = controlRoom.AddStreet("C", "B", 20, 60);
            Assert.IsNotNull(street2);

            Assert.IsTrue(controlRoom.PairStreets("A", "B", "C"));
            Tuple<ISignal, ISignal> streetPair = Tuple.Create<ISignal, ISignal>(signal, signal3);
            signal2.StreetPairs.Contains(streetPair);
        }

        [TestMethod]
        public void TestPairStreetsBadInput()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            ISignal signal2 = controlRoom.AddSignal("B");
            Assert.IsNotNull(signal2);

            ISignal signal3 = controlRoom.AddSignal("C");
            Assert.IsNotNull(signal3);

            IStreet street = controlRoom.AddStreet("A", "D", 10, 60);
            Assert.IsNull(street);            
        }

        [TestMethod]
        public void TestGetFastestRouteOnSingleNode()
        {
            ITrafficControlRoom controlRoom = new TrafficControlRoom(SpecialDijkstra.GetInstance());
            ISignal signal = controlRoom.AddSignal("A");
            Assert.IsNotNull(signal);

            IRoute route = controlRoom.GetFastestRoute("A", "A", null);
            Assert.IsNotNull(route);
            Assert.AreEqual(route.Path.Count, 1);
            Assert.AreEqual(route.Path[0].Name, "A");
        }        
    }
}

