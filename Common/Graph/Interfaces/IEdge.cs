namespace Common.Graph.Interfaces
{
    public interface IEdge
    {
        IVertex StartVertex { get; set; }
        IVertex DestinationVertex { get; set; }
        double Weight { get; set; }
    }
}
