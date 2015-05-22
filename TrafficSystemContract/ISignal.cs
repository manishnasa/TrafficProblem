using System;
using System.Collections.Generic;
using Common.Graph.Interfaces;

namespace TrafficSystemContract
{
    public interface ISignal : IVertex
    {
        TimeSpan WaitTime { get; set; }
        TimeSpan PassthroughTimePerCar { get; set; }
        int NumberOfCarsWaiting { get; set; }
    }
}
