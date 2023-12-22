using System;
using Chamberlain.AppServer.Api.Contracts.Commands.Profiles;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class ProfileActor : Receiver
    {
        [SetterProperty]
        public IProfileService ProfileService { get; set; }

        public ProfileActor()
        {

            Receive<GetProfiles>(msg =>
            {
                Context.Handle(msg, item => ProfileService.GetProfiles(item.UserName));
            });
            Receive<GetProfile>(msg =>
            {
                Context.Handle(msg, item => ProfileService.GetProfile(item.UserName, item.ProfileId));
            });
            Receive<AddProfile>(msg =>
            {
                Context.Handle(msg, item => ProfileService.AddProfile(item.UserName, item.ProfileModel));
            });
            Receive<UpdateProfile>(msg =>
            {
                Context.Handle(msg, item => ProfileService.UpdateProfile(item.UserName, item.ProfileModel));
            });
            Receive<DeleteProfile>(msg =>
            {
                Context.Handle(msg, item => ProfileService.DeleteProfile(item.UserName, item.ProfileId));
            });
            Receive<AddPhoto>(msg =>
            {
                Context.Handle(msg, item => ProfileService.AddImage(item.UserName, item.ProfileId, item.ModelType));
            });
            Receive<StartModelTraining>(msg =>
            {
                Context.Handle(msg, AddActorToMessage);
            });
            Receive<CheckFaceViewReadiness>(msg =>
            {
                Context.Handle(msg, item => ProfileService.CheckFaceViewReadiness(item.UserName, item.ProfileId, item.ModelType));
            });
            Receive<UploadSamples>(msg => {
                Context.Handle(msg, item => ProfileService.UploadSamples(item.UserName));
            });
        }

        private void AddActorToMessage(StartModelTraining item)
        {
            var overseerActor = Context.System
                .ActorSelection("user/username/overseer".Replace("{username}", item.UserName))
                .ResolveOne(TimeSpan.FromSeconds(10)).Result;

            ProfileService.StartModelTraining(item.UserName, item.ProfileId, overseerActor);
        }
    }
}
