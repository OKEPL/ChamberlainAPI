using System.Collections.Generic;
using System.Linq;

using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Helpers
{
    public static class LargePacketMessageHelper
    {
        public static List<T> Split<T>(T msg, int maxFrameSize) where T : ILargePacketMessage
        {
            int totalParts = (int)decimal.Ceiling(msg.Data.Length / (decimal)maxFrameSize);
            List<T> result = new List<T>();
            int readed = 0;
            while (readed < msg.Data.Length)
            {
                var msgTemp = msg.Copy();
                msgTemp.TotalParts = totalParts;
                msgTemp.Data = msg.Data.Skip(readed).Take(maxFrameSize).ToArray();
                msgTemp.PartNumber = result.Count;
                result.Add((T)msgTemp);
                readed += msgTemp.Data.Length;
            }
            return result;
        }
    }
}
