using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Akka.Actor;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.Plugins.HlsHandler;
using global::AppServer.Api.Endpoint.Controllers;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using static Chamberlain.AppServer.Api.Hubs.Contracts.Commands.CameraDataTransfer;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{

    /// <inheritdoc />
    /// <summary>
    /// Chunk controller class.
    /// </summary>
    [Route("chunks")]
    public class ChunkController : ChamberlainBaseController
    {
        private static ChunkServices _services = new ChunkServices();

        /// <summary>
        /// Adding chunks from camera
        /// </summary>
        /// <param name="chunk">Chunk to be added</param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Add([FromBody] SendTsChunk chunk)
        {
            try
            {
                if (!_services.ContainsKey(chunk.DeviceKey))
                {
                    _services.Add(chunk.DeviceKey);
                }
               await _services.SaveChunk(chunk);
            }
            catch (Exception exc)
            {
                Log.Error(exc, "There was an error when trying to save chunk.");
            }
            return NoContent();
        }

        private class ChunkServices : Dictionary<string, HlsStreamlinedService>
        {
            public void Add(string deviceKey)
            {
                var actorRef = SystemActors.ChunkActor.ResolveOne(System.TimeSpan.FromSeconds(10)).Result;
                var response = actorRef.Ask<GetChunkSettingsResponse>(new GetChunkSettings(deviceKey)).Result;

                Add(deviceKey, new HlsStreamlinedService(deviceKey, response.MainPath, response.RecordingPath, response.PreviewPath, response.HubCameraListSize, response.ChunkSecondsLength));
            }

            public async Task SaveChunk(SendTsChunk chunk)
            {
                var processingResult = await base[chunk.DeviceKey].ProcessChunk(chunk.ChunkName, chunk.Data, chunk.Timestamp, chunk.Extinf);
                if (processingResult)
                {
                    SystemActors.ChunkActor.Tell(new UpdateCamera(chunk.DeviceKey));
                }
            }
        }
    }
}