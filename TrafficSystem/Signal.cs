using System;
using System.Collections.Generic;
using Common.Graph;
using Common.Graph.Interfaces;
using Common.Logger.Interfaces;
using Common.Logger;
using TrafficSystemContract;

namespace TrafficSystem
{
    class Signal: Vertex, ISignal
    {

        #region Properties
        
        public Dictionary<IVertex, int> TrafficInformation { get; set; }           
        public TimeSpan WaitTime { get; set; }      
        public TimeSpan PassthroughTimePerCar { get; set; }
        public IList<Tuple<ISignal, ISignal>> StreetPairs { get; set; }
        ILogger Logger;
              
        #endregion

        #region Constructors

        public Signal(string name): base(name)
        {
            WaitTime = new TimeSpan(0, 0, 0);
            PassthroughTimePerCar = new TimeSpan(0, 0, 0);
            Logger = DumbConsoleLogger.GetInstance();
            StreetPairs = new List<Tuple<ISignal, ISignal>>();
        }

        public Signal(string name, int waitTimeInseconds, int passThroughTimeInSeconds)
            : this(name)
        {
            if (waitTimeInseconds < 0 || passThroughTimeInSeconds < 0)
                throw new ArgumentException("Bad argument waitTimeInseconds or passThroughTimeInSeconds in Signal constructor.");

            WaitTime = new TimeSpan(0, 0, waitTimeInseconds);
            PassthroughTimePerCar = new TimeSpan(0, 0, passThroughTimeInSeconds);
        }

        #endregion

        int GetNumberOfConnectingSignals()
        {
            return Adjacencies.Count;
        }

        //Unused method.
        //What if the signal was traffic independent ? not asked, not used - but just for curiosity! :)
        public double GetWeight2(IVertex startSignal)
        {
            if (PassthroughTimePerCar.Seconds == 0 && WaitTime.Seconds == 0)
            {
                return 0; //It's a free signal
            }
            else if (PassthroughTimePerCar.Seconds > 0 && WaitTime.Seconds == 0)
            {
                return PassthroughTimePerCar.Seconds; //no wait time at signal, but a car needs X amount of time to get through
            }
            else
            {
                int maxCarsInOneGo = WaitTime.Seconds / PassthroughTimePerCar.Seconds;
                Logger.Info("A maximum of " + maxCarsInOneGo + " cars can go through this signal at one time.");

                int numCarsWaitingOnMyStreet;
                if (!TrafficInformation.TryGetValue(startSignal, out numCarsWaitingOnMyStreet))
                {
                    return 0;
                }

                //Assuming that when a car comes to a signal, it turns green only after
                //all other signals have had their turns
                //We divide by 2, at one time - two opposite streets open up.
                int numberOfSignalsBeforeMine = GetNumberOfConnectingSignals() / 2 - 1;
                Logger.Info("There are " + numberOfSignalsBeforeMine + " more signals other than mine at this junction.");

                int numberOfTurns = numCarsWaitingOnMyStreet / maxCarsInOneGo + 1;
                Logger.Info("It will take " + numberOfTurns + " turns for me to get through.");
                int myTurnInMyGo = numCarsWaitingOnMyStreet % maxCarsInOneGo;
                Logger.Info("In my turn " + myTurnInMyGo + " cars will go through before me.");

                int waitTimeForOtherSignals = numberOfSignalsBeforeMine * WaitTime.Seconds * numberOfTurns;
                int waitTimeForMySignal = (numberOfTurns - 1) * WaitTime.Seconds;
                int waitTimeToGetThroughInMyTurn = myTurnInMyGo * PassthroughTimePerCar.Seconds;

                int totalWaitTime = waitTimeForOtherSignals + waitTimeForMySignal + waitTimeToGetThroughInMyTurn;

                Logger.Info("I will have to wait for " + totalWaitTime + " seconds at this signal.");

                return totalWaitTime;
            }
        }

        override public double GetWeight(IVertex startSignal)
        {
            if( TrafficInformation == null )
            {
                return 0;
            }
            else if( PassthroughTimePerCar.Seconds == 0 && WaitTime.Seconds == 0 )
            {
                return 0; //It's a free signal
            }           
            else if (PassthroughTimePerCar.Seconds > 0 && WaitTime.Seconds == 0)
            {
                return PassthroughTimePerCar.Seconds; //no wait time at signal, but a car needs X amount of time to get through
            }
            else
            {                
                int maxCarsInOneGo = WaitTime.Seconds / PassthroughTimePerCar.Seconds;
                Logger.Info("A maximum of " + maxCarsInOneGo + " cars can go through this signal at one time.");

                //Assuming that when a car comes to a signal, it turns green only after
                //all other signals have had their turns

                //1. Calculate wait time at my signal (like if there were no other signals)
                int numCarsWaitingOnMyStreet;
                if (!TrafficInformation.TryGetValue(startSignal, out numCarsWaitingOnMyStreet))
                {
                    return 0;
                }

                int numberOfTurns = numCarsWaitingOnMyStreet / maxCarsInOneGo + 1;
                Logger.Info("It will take " + numberOfTurns + " turns for me to get through.");
                int myTurnInMyGo = numCarsWaitingOnMyStreet % maxCarsInOneGo;
                Logger.Info("In my turn " + myTurnInMyGo + " cars will go through before me.");                

                //multiply by 2 to support the 30 sec red light in each turn - as per the question.
                int waitTimeForMySignal = (numberOfTurns - 1) *  WaitTime.Seconds * 2;
                int waitTimeToGetThroughInMyTurn = myTurnInMyGo * PassthroughTimePerCar.Seconds;

                //2. Wait time at other signals
                var trafficStatus = TrafficInformation;

                int waitTimeForOtherSignals = 0;
                for(int i=0; i< numberOfTurns; i++)
                {
                    //for each turn, add time for each signal
                    //some might be 0 - if there is no traffic there
                    //because this is a traffic dependent system

                    foreach( var streetPair in StreetPairs )
                    {
                        if (streetPair.Item1 == startSignal || streetPair.Item2 == startSignal)
                            continue; //time for mySignal is already calculate above.

                        int numCarsAtStreet1 =0, numCarsAtStreet2 = 0;
                        trafficStatus.TryGetValue(streetPair.Item1, out numCarsAtStreet1);
                        trafficStatus.TryGetValue(streetPair.Item2, out numCarsAtStreet2);

                        if(numCarsAtStreet1 == 0 &&  numCarsAtStreet2 == 0)
                        {
                            continue; //no traffic on this street pair, so no redlight and consequently no waiting for this one.
                        }

                        waitTimeForOtherSignals += 2 * WaitTime.Seconds; 

                        numCarsAtStreet1 = numCarsAtStreet1 - maxCarsInOneGo;
                        if( numCarsAtStreet1 < 0 ) numCarsAtStreet1 = 0;

                        numCarsAtStreet2 = numCarsAtStreet2 - maxCarsInOneGo;
                        if( numCarsAtStreet2 < 0 ) numCarsAtStreet2 = 0;

                        trafficStatus[streetPair.Item1] = numCarsAtStreet1;
                        trafficStatus[streetPair.Item2] = numCarsAtStreet2;
                    }

                }

                Logger.Info("There are " + (StreetPairs.Count-1) + " more signals other than mine at this junction.");
                                
                int totalWaitTime = waitTimeForOtherSignals + waitTimeForMySignal + waitTimeToGetThroughInMyTurn;

                Logger.Info("I will have to wait for " + totalWaitTime + " seconds at this signal.");

                return totalWaitTime;
            }
        }              
    }
}
