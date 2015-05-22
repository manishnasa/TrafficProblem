using System;
using System.Collections.Generic;
using Common.Graph;
using Common.Graph.Interfaces;
using TrafficSystemContract;

namespace TrafficSystem
{
    public class Signal: Vertex, ISignal
    {
        public Signal(string name): base(name)
        {
            WaitTime = new TimeSpan(0, 0, 0);
            PassthroughTimePerCar = new TimeSpan(0, 0, 0);
            NumberOfCarsWaiting = 0;            
        }

        public Signal(string name, int waitTimeInseconds, int passThroughTimeInSeconds)
            : base(name)
        {
            if (waitTimeInseconds < 0 || passThroughTimeInSeconds < 0)
                throw new ArgumentException("Bad argument waitTimeInseconds or passThroughTimeInSeconds in Signal constructor.");

            WaitTime = new TimeSpan(0, 0, waitTimeInseconds);
            PassthroughTimePerCar = new TimeSpan(0, 0, passThroughTimeInSeconds);
            NumberOfCarsWaiting = 0;
        }

        #region Properties

        TimeSpan waitTime;
        public TimeSpan WaitTime
        {
            get
            {
                return waitTime;
            }
            set //update weight, when WaitTime is updated
            {
                waitTime = value;
                Weight = CalculateWeight();
            }
        }

        TimeSpan passthroughTime;
        public TimeSpan PassthroughTimePerCar 
        { 
            get
            {
                return passthroughTime;
            }
            set //update weight, when PassthroughTime is updated
            {
                passthroughTime = value;
                Weight = CalculateWeight();
            }
        }

        int numCarsWaiting;
        public int NumberOfCarsWaiting
        {
            get
            {
                return numCarsWaiting;
            }
            set //update weight, when NumberOfCarsWaiting is updated
            {
                numCarsWaiting = value;
                Weight = CalculateWeight();
            }
        }

        #endregion

        double CalculateWeight()
        {
            if( PassthroughTimePerCar.Seconds == 0 && WaitTime.Seconds == 0 )
            {
                return 0; //It's a free signal
            }
            else if( PassthroughTimePerCar.Seconds == 0 && WaitTime.Seconds > 0 )
            {
                return WaitTime.Seconds; //A case where the signal has a fixed wait time, irrespective of traffic
            }
            else if (PassthroughTimePerCar.Seconds > 0 && WaitTime.Seconds == 0)
            {
                return PassthroughTimePerCar.Seconds; //no wait time at signal, but a car beeds X amount of time to get through
            }
            else
            {
                int maxCarsInOneGo = WaitTime.Seconds / PassthroughTimePerCar.Seconds;
                int numberOfTurnsToWait = NumberOfCarsWaiting / maxCarsInOneGo;
                int myTurnInMyGo = NumberOfCarsWaiting % maxCarsInOneGo;

                //multiply by two - because cars go through for WaitTime (Green light), and wait for WaitTime (Red light)
                return (numberOfTurnsToWait * WaitTime.Seconds * 2) + (myTurnInMyGo * PassthroughTimePerCar.Seconds);
            }
        }        
    }
}
