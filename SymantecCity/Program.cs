using System;
using System.Collections.Generic;
using TrafficSystemContract;
using TrafficSystem;

namespace SymantecCity
{
    class Program
    {
        static void Main(string[] args)
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
            Console.ReadKey();
        }
    }
}
