using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Data_Structures
{
    public class PartialDataContainer
    {
        public int SlicesCount { get; set; }
        public Dictionary<int, Slice> slicesById { get; set; }
        private static readonly object Lock = new object();
        public PartialDataContainer(int slicesCount)
        {
            SlicesCount = slicesCount;
            slicesById = new Dictionary<int, Slice>();
        }
        public void InsertSlice(int sliceId, byte[] data)
        {
            lock (Lock)
            {
                slicesById[sliceId] = new Slice
                {
                    SliceData = data,
                    SliceSize = data.Length
                };
            }
        }

        public bool IsComplete()
        {
            for(var i = 0; i < SlicesCount; i++)
                if (!slicesById.ContainsKey(i))
                    return false;

            return true;
        }

        private int GetTotalLength()
        {
            var result = 0;
            foreach (var slice in slicesById)
            {
                result += slice.Value.SliceSize;
            }
            return result;
        }

        public byte[] GetCompleteData()
        {
            var result = new byte[GetTotalLength()];

            for (int i = 0; i < SlicesCount-1; i++)
            {
                slicesById[i].SliceData.CopyTo(result, i*slicesById[i].SliceSize);
            }
            slicesById[SlicesCount-1].SliceData.CopyTo(result, result.Length - slicesById[SlicesCount-1].SliceSize);
            return result;
        }
    }
}
