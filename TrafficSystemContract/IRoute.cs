using System;
using System.Collections.Generic;
using TrafficSystemContract;

namespace TrafficSystemContract
{
    public interface IRoute
    {
        IList<ISignal> Path { get; set; }
    }
}
