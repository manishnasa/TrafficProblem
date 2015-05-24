using System;
using System.Collections.Generic;
using Common.Graph;
using TrafficSystemContract;

namespace TrafficSystem
{
    class Street: Edge, IStreet
    {
        public Street(ISignal startVertex, ISignal destinationVertex, double distanceInKm, double speedInKmph)
            : base(startVertex, destinationVertex)
        {
            if (distanceInKm < 0 || speedInKmph <= 0)
                throw new ArgumentException("Invalid arguments in Street constructor.");

            Weight = (distanceInKm / speedInKmph) * 3600; //*3600 to convert hours to seconds 
        }
    }
}
