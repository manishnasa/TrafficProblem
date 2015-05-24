using System;
using System.Collections.Generic;
using TrafficSystemContract;
using Common.Graph.Interfaces;
using Common.Graph;
using Common.Logger.Interfaces;
using Common.Logger;
using Wintellect.PowerCollections;

namespace TrafficSystem
{
    public class TrafficControlRoom: ITrafficControlRoom
    {
        #region Properties

        public  Set<ISignal> Signals { get; set; }

        IShortestPathAlgorithm ShortestPathAlgorithm;
        ILogger Logger;

        #endregion

        #region Private

        ISignal GetSignal(string name)
        {
            ISignal findSignal = new Signal(name);
            ISignal signal = null;
            Signals.TryGetItem(findSignal, out signal);

            return signal;
        }

        IList<ISignal> VertexListToSignalList(IList<IVertex> vertexPath)
        {
            if (vertexPath == null)
            {
                Logger.Error("Invalid vertexPath in VertexListToSignalList.");
                return null;
            }

            IList<ISignal> signalPath = new List<ISignal>();
            foreach (var vertex in vertexPath)
            {
                ISignal signal = GetSignal(vertex.Name);
                if (signal == null)
                {
                    Logger.Error("Invalid data. Shortest path contains a signal: " + vertex.Name + " that does not exist.");
                    return null;
                }

                signalPath.Add(signal);
            }

            return signalPath;
        }

        void UpdateSignalsWithTrafficInformation(Dictionary<string, Dictionary<string, int>> allSignalTraffic)
        {            
            foreach (var signalName in allSignalTraffic.Keys)
            {
                var signal = GetSignal(signalName);
                if (signal == null)
                {
                    Logger.Error("Invalid signalName: + " + signalName + " in Input.");
                    continue;
                }

                Dictionary<string, int> trafficInfoAtSignal;
                allSignalTraffic.TryGetValue(signalName, out trafficInfoAtSignal);
                Dictionary<IVertex, int> trafficInfoAtSignalWithSignalObject = new Dictionary<IVertex, int>();

                foreach (var trafficInfoSignalName in trafficInfoAtSignal.Keys)
                {
                    var trafficInfoSignal = GetSignal(trafficInfoSignalName);
                    if (trafficInfoSignal == null)
                    {
                        Logger.Error("Invalid signalName: + " + trafficInfoSignal + " in Input.");
                        continue;
                    }

                    int traffic;
                    trafficInfoAtSignal.TryGetValue(trafficInfoSignalName, out traffic);
                    trafficInfoAtSignalWithSignalObject.Add(trafficInfoSignal, traffic);
                }

                signal.TrafficInformation = trafficInfoAtSignalWithSignalObject;
            }
        }

        #endregion

        public TrafficControlRoom(IShortestPathAlgorithm shortestPathAlgorithm)
        {
            if (shortestPathAlgorithm == null)
                throw new ArgumentNullException("Null argument in TrafficControlRoom constructor");

            Signals = new Set<ISignal>();
            ShortestPathAlgorithm = shortestPathAlgorithm;
            Logger = DumbConsoleLogger.GetInstance();
        }

        public ISignal AddSignal(string name, int waitTimeInSeconds = 0, int passThroughTime = 0)
        {
            if(string.IsNullOrEmpty(name))
            {
                Logger.Error("That was an empty name! Cannot create signal.");
                return null;
            }

            if(waitTimeInSeconds < 0 || passThroughTime < 0)
            {
                Logger.Error("Negative waitTime or passThroughTime.");
                return null;
            }

            ISignal signal = new Signal(name, waitTimeInSeconds, passThroughTime);            
            if( Signals.Add(signal) )
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

            if(startName == destinationName)
            {
                Logger.Error("Cannot make street from same point to same point!");
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

        //At a junction or signal, streets opposite each other will be called PairedStreets
        //In our example - EX - XC and AX - XB will be pair streets
        //This information is important because traffic would clear from both the pair streets in one green light round
        public bool PairStreets(string point1Name, string junctionName, string point2Name)
        {
            ISignal point1 = GetSignal(point1Name);
            ISignal junction = GetSignal(junctionName);
            ISignal point2 = GetSignal(point2Name);

            if( point1 == null || junction == null || point2 == null )
            {
                Logger.Error("One of the input signals doesnt exist. Invalid input to PairStreets!");
                return false;
            }

            Tuple<ISignal, ISignal> streetPair = Tuple.Create<ISignal, ISignal>(point1, point2);
            junction.StreetPairs.Add(streetPair);

            return true;            
        }

        public IRoute GetFastestRoute(string startName, string destinationName, Dictionary<string, Dictionary<string, int>> allSignalTraffic)
        {
            ISignal startSignal = GetSignal(startName);
            ISignal destinationSignal = GetSignal(destinationName);
            if (startSignal == null || destinationSignal == null)
            {
                Logger.Error("Cannot get faster route! At least one of the input signals doesnt exist.");
                return null;
            }

            if (allSignalTraffic != null)
                UpdateSignalsWithTrafficInformation(allSignalTraffic);
            
            IList<IVertex> vertexPath = ShortestPathAlgorithm.GetShortestPath(startSignal, destinationSignal);
            Logger.Info("\n\nThe fastest path would be: ");
            foreach( var vertex in vertexPath )
            {
                Logger.Info(vertex.Name);
            }

            return new Route(VertexListToSignalList(vertexPath));
        }
        
    }
}
