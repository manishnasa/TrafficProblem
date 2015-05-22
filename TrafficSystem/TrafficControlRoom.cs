using System;
using System.Collections.Generic;
using TrafficSystemContract;
using Common.Graph.Interfaces;
using Common.Graph;
using Common.Logger.Interfaces;
using Wintellect.PowerCollections;

namespace TrafficSystem
{
    public class TrafficControlRoom: ITrafficControlRoom
    {
        #region Properties

        Set<ISignal> Signals { get; set; }
        Set<IStreet> Streets { get; set; }

        IShortestPathAlgorithm ShortestPathAlgorithm;
        ILogger Logger;

        #endregion

        public TrafficControlRoom(IShortestPathAlgorithm shortestPathAlgorithm, ILogger logger)
        {
            if (shortestPathAlgorithm == null || logger == null)
                throw new ArgumentNullException("Null argument in TrafficControlRoom constructor");

            Signals = new Set<ISignal>();
            Streets = new Set<IStreet>();
            ShortestPathAlgorithm = shortestPathAlgorithm;
            Logger = logger;
        }

        public ISignal AddSignal(string name, int waitTimeInSeconds = 0, int passThroughTime = 0)
        {
            if(string.IsNullOrEmpty(name))
            {
                Logger.Error("That was an empty name! Cannot create signal.");
                return null;
            }

            ISignal signal = new Signal(name, waitTimeInSeconds, passThroughTime);            
            if( !Signals.Add(signal) )
            {
                Logger.Error("Oops, a Signal with name " + name + " already exists!");
                return null;
            }

            return signal;
        }

        public IStreet AddStreet(string startName, string destinationName, double distanceInKm, double speedInKmph)
        {
            if( distanceInKm < 0 || speedInKmph < 0 )
            {
                Logger.Error("That was a negative distance or speed!");
                return null;
            }

            ISignal startSignal = GetSignal(startName);
            ISignal destinationSignal = GetSignal(destinationName);
            if (startSignal == null || destinationSignal == null)
            {
                Logger.Error("Cannot create a street with signals " + startName + " and " + destinationName + ". At least one of them does not exist!");
                return null;
            }

            IStreet street = new Street(startSignal, destinationSignal, distanceInKm, speedInKmph);            
            return street;
        }

        public bool AddDoubleStreet(string point1, string point2, double distanceInKm, double speedInKmph)
        {
            IStreet street1 = AddStreet(point1, point2, distanceInKm, speedInKmph);
            if (street1 == null) return false;

            IStreet street2 = AddStreet(point2, point1, distanceInKm, speedInKmph);
            if (street2 == null) return false;
             
            return true;
        }

        public IRoute GetFastestRoute(string startName, string destinationName, Dictionary<string, int> signalsWithTrafficCount)
        {
            ISignal startSignal = GetSignal(startName);
            ISignal destinationSignal = GetSignal(destinationName);
            if (startSignal == null || destinationSignal == null)
            {
                Logger.Error("Cannot get faster route! At least one of the input signals doesnt exist.");
                return null;
            }

            //Update the traffic information on the signals
            if (signalsWithTrafficCount != null )
            {
                foreach (var signalName in signalsWithTrafficCount.Keys)
                {
                    int num;
                    if (!signalsWithTrafficCount.TryGetValue(signalName, out num))
                    {
                        //This should ideally never happen
                        Logger.Error("Something is not right. Invalid data in signalsWithTrafficCount in TrafficControlRoom:GetFastestRoute");
                        return null;
                    }

                    ISignal signal = GetSignal(signalName);
                    if (signal == null)
                    {
                        Logger.Error("Bad data in signalsWithTrafficCount in TrafficControlRoom:GetFastestRoute. Signal " + signalName + " does not exist.");
                        return null;
                    }
                    signal.NumberOfCarsWaiting = num;
                }
            }
            
            IList<IVertex> vertexPath = ShortestPathAlgorithm.GetShortestPath(startSignal, destinationSignal);
            return new Route(VertexListToSignalList(vertexPath));
        }

        ISignal GetSignal(string name)
        {
            ISignal findSignal = new Signal(name);
            ISignal signal = null;
            Signals.TryGetItem(findSignal, out signal);

            return signal;
        }

        IList<ISignal> VertexListToSignalList(IList<IVertex> vertexPath)
        {
            if( vertexPath == null )
            {
                Logger.Error("Invalid vertexPath in VertexListToSignalList.");
                return null;
            }

            IList<ISignal> signalPath = new List<ISignal>();
            foreach(var vertex in vertexPath)
            {
                ISignal signal = GetSignal(vertex.Name);
                if( signal == null )
                {
                    Logger.Error("Invalid data. Shortest path contains a signal: " + vertex.Name + " that does not exist.");
                    return null;
                }

                signalPath.Add(signal);
            }

            return signalPath;
        }
    }
}
