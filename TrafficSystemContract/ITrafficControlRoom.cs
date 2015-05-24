using System;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace TrafficSystemContract
{
    public interface ITrafficControlRoom
    {
        Set<ISignal> Signals { get; set; }

        ISignal AddSignal(string name, int waitTimeInSeconds = 0, int passThroughTime = 0);
        IStreet AddStreet(string startName, string destinationName, double distanceInKm, double speedInKmph);
        bool AddDoubleStreet(string point1, string point2, double distanceInKm, double speedInKmph);
        bool PairStreets(string point1Name, string junctionName, string point2Name);
        IRoute GetFastestRoute(string startName, string destinationName, Dictionary<string, Dictionary<string, int>> allSignalTraffic);
    }
}
