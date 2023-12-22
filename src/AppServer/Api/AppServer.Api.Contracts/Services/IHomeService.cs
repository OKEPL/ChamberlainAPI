namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;

    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Device.Camera;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Event;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Status;

    #endregion

    public interface IHomeService
    {
        void ClearEvents(string userName);

        CameraImageModel GetCameraImageByThingId(string userName, long thingId);
        
        List<CameraModel> GetCamerasWithImages(string userName);

        List<EventModel> GetEventsFromBegining(string userName, int number, string language);

        List<EventModel> GetEventsFromId(string userName, int number, int lastSeen, string language);

        List<EventModel> GetNewestEventsFromId(string userName, int lastSeen, string language);

        StatusModel GetStatus(string userName);
    }
}