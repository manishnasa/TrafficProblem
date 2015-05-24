using System;
using System.Collections.Generic;


namespace TrafficSystemContract
{
    public interface ITrafficControlRoom
    {
        ISignal AddSignal(string name, int waitTimeInSeconds = 0, int passThroughTime = 0);
        IStreet AddStreet(string startName, string destinationName, double distanceInKm, double speedInKmph);
        bool AddDoubleStreet(string point1, string point2, double distanceInKm, double speedInKmph);
        IRoute GetFastestRoute(string startName, string destinationName, Dictionary<string, Dictionary<string, int>> allSignalTraffic);
    }
}
