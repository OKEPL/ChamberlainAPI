namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public interface ILargePacketMessage
    {
        byte[] Data { get; set; }
        int PartNumber { get; set; }
        int TotalParts { get; set; }
        long Timestamp { get; }
        ILargePacketMessage Copy();

    }
}
