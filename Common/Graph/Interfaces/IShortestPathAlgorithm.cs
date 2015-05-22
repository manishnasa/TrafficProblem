using System.Collections.Generic;

namespace Common.Graph.Interfaces
{
    public interface IShortestPathAlgorithm
    {
        IList<IVertex> GetShortestPath(IVertex source, IVertex destination);
    }
}
