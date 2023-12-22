namespace Chamberlain.AppServer.Api.Endpoint.Helpers
{
    #region

    using Akka.Actor;

    using Microsoft.Extensions.Configuration;

    #endregion
    #pragma warning disable
    public class ActorBootstrapper
    {
        public ActorBootstrapper(ActorSystem system, IConfiguration configuration)
        {
            var remoteSystemUrl = configuration["Akka:AppServerUrl"];
            SystemActors.CameraActor = system.ActorSelection($"{remoteSystemUrl}/user/cameras");
            SystemActors.ScheduleActor = system.ActorSelection($"{remoteSystemUrl}/user/schedules");
            SystemActors.ModeActor = system.ActorSelection($"{remoteSystemUrl}/user/modes");
            SystemActors.IftttActor = system.ActorSelection($"{remoteSystemUrl}/user/ifttts");

            SystemActors.CustomerActor = system.ActorSelection($"{remoteSystemUrl}/user/customers");
            SystemActors.CustomerEmailActor = system.ActorSelection($"{remoteSystemUrl}/user/customeremails");
            SystemActors.CustomerIftttActor = system.ActorSelection($"{remoteSystemUrl}/user/customerifttt");

            SystemActors.CustomerPushActor = system.ActorSelection($"{remoteSystemUrl}/user/customerpush");
            SystemActors.CustomerSmsActor = system.ActorSelection($"{remoteSystemUrl}/user/customersms");
            SystemActors.RecordingActor = system.ActorSelection($"{remoteSystemUrl}/user/recordings");
            SystemActors.StatisticsActor = system.ActorSelection($"{remoteSystemUrl}/user/statistics");
            SystemActors.DeviceActor = system.ActorSelection($"{remoteSystemUrl}/user/devices");

            SystemActors.RuleActor = system.ActorSelection($"{remoteSystemUrl}/user/rules");
            SystemActors.ClockTriggerActor = system.ActorSelection($"{remoteSystemUrl}/user/clocktriggers");
            SystemActors.AuthActor = system.ActorSelection($"{remoteSystemUrl}/user/auth");
            SystemActors.FeatureActor = system.ActorSelection($"{remoteSystemUrl}/user/features");
            SystemActors.HomeActor = system.ActorSelection($"{remoteSystemUrl}/user/home");
            SystemActors.HubActor = system.ActorSelection($"{remoteSystemUrl}/user/hubs");
            SystemActors.TokenActor = system.ActorSelection($"{remoteSystemUrl}/user/tokens");
            SystemActors.ProfileActor = system.ActorSelection($"{remoteSystemUrl}/user/profiles");
            SystemActors.VoiceActor = system.ActorSelection($"{remoteSystemUrl}/user/voice");
            SystemActors.SceneActor = system.ActorSelection($"{remoteSystemUrl}/user/scenes");
            SystemActors.TestActor = system.ActorSelection($"{remoteSystemUrl}/user/tests");
            SystemActors.StbActor = system.ActorSelection($"{remoteSystemUrl}/user/stb");
            SystemActors.ChunkActor = system.ActorSelection($"{remoteSystemUrl}/user/chunks");
        }
    }

    public static class SystemActors
    {
        public static ActorSelection AuthActor;

        public static ActorSelection CameraActor;

        public static ActorSelection ClockTriggerActor;

        public static ActorSelection CustomerActor;

        public static ActorSelection CustomerEmailActor;

        public static ActorSelection CustomerIftttActor;

        public static ActorSelection CustomerPushActor;

        public static ActorSelection CustomerSmsActor;

        public static ActorSelection DeviceActor;

        public static ActorSelection FeatureActor;

        public static ActorSelection HomeActor;

        public static ActorSelection HubActor;

        public static ActorSelection IftttActor;

        public static ActorSelection ModeActor;

        public static ActorSelection RecordingActor;

        public static ActorSelection RuleActor;

        public static ActorSelection ScheduleActor;

        public static ActorSelection StatisticsActor;

        public static ActorSelection TokenActor;

        public static ActorSelection ProfileActor;

        public static ActorSelection VoiceActor;

        public static ActorSelection SceneActor;

        public static ActorSelection TestActor;
		
        public static ActorSelection StbActor;

        public static ActorSelection ChunkActor;
    }
}