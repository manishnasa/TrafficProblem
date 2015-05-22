using System;
using System.Collections.Generic;
using TrafficSystemContract;

namespace TrafficSystem
{
    class Route: IRoute
    {
        public IList<ISignal> Path { get; set; }      
        public Route(IList<ISignal> path)
        {
            Path = path;
        }       
    }
}
