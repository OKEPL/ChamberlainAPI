namespace Chamberlain.AppServer.Api.Contracts.Services
{
    #region

    using System.Collections.Generic;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
    using Akka.Actor;

    #endregion

    public interface IModeService
    {
        void AddMode(string userName, ModePostModel model, IActorRef ActorRef);
        
        void DeleteMode(string userName, long modeId, IActorRef ActorRef);

        ModeModel GetMode(string userName, long modeId);

        List<ModeModel> GetModes(string userName);

        void Update(string userName, ModeModel model, IActorRef ActorRef);
        
        void UpdateMode(string userName, long modeId, string name);

        void UpdateModeColor(string userName, long modeId, string color);
    }
}