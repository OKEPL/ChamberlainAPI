using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Helpers
{
    public class LargePacketMessageSpooler
    {
        private static readonly List<ILargePacketMessage> splitedData = new List<ILargePacketMessage>();
        private static object _lock = new object();
        private static LargePacketMessageSpooler _instance;

        public static LargePacketMessageSpooler Instance
        {
            get
            {
                if (_instance == null)
                {
                    return _instance = new LargePacketMessageSpooler();
                }

                return _instance;
            }

        }

        public void HandleLargePacketMessage(ILargePacketMessage msg, Action<object> callback)
        {
            lock (_lock)
            {
                splitedData.Add(msg);

                if (splitedData.Count(c => c.Timestamp == msg.Timestamp) != msg.TotalParts)
                    return;

                var allParts = splitedData.Where(s => s.Timestamp == msg.Timestamp).OrderBy(o => o.PartNumber).ToList();
                List<byte> allData = new List<byte>();
                allParts.ForEach(a => { splitedData.Remove(a); allData.AddRange(a.Data); });
                msg.Data = allData.ToArray();
                allParts.Clear();
                allData.Clear();

                //if (msg is KinectDataTrasnfer.SendFrameData k)
                //{
                //    k.KinectData = new ushort[k.Data.Length / 2];
                //    Buffer.BlockCopy(k.Data, 0, k.KinectData, 0, k.KinectData.Length);
                //    k.Data = null;
                //}
            }
            callback?.Invoke(msg);

        }

    }
}
