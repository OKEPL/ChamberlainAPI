using System;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.Common.Content.DataContracts;

namespace Chamberlain.AppServer.Api.Hubs.Reporters
{
    public interface IReporter : IDisposable
    {
        long ReportThing(IoTThingData thingData);
        long ReportItem(HubWorkerCommand.ReportItem itemData);
        void RecognizeItemIfNeeded();
        void ReportDeviceReadyForCommands();
    }
}
