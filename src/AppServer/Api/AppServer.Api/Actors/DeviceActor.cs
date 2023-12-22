using Chamberlain.AppServer.Api.Contracts.Commands.Devices;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Akka;
using StructureMap.Attributes;

namespace Chamberlain.AppServer.Api.Actors
{
    public class DeviceActor : Receiver
    {
        [SetterProperty]
        public IDeviceService DeviceService { get; set; }

        public DeviceActor()
        {
			Receive<GetDevices>(msg => {
				Context.Handle(msg, item => DeviceService.GetDevices(item.UserName));
				});
            Receive<GetDevicesThingNames>(msg => {
                Context.Handle(msg, item => DeviceService.GetDevicesThingNames(item.UserName));
            });
            Receive<GetDevice>(msg => {
				Context.Handle(msg, item => DeviceService.GetDevice(item.UserName, item.DeviceId));
				});
			Receive<GetAvaliableTriggers>(msg => {
				Context.Handle(msg, item => DeviceService.GetAvaliableTriggers(item.UserName, item.DeviceId));
				});
			Receive<UpdateDevice>(msg => {
				Context.Handle(msg, item => DeviceService.UpdateDevice(item.UserName, item.DeviceId, item.DeviceName));
				});
			Receive<DeleteDeviceByThingName>(msg => {
				Context.Handle(msg, item => DeviceService.DeleteDeviceByThingId(item.UserName, item.ThingId));
				});
			Receive<PairDevices>(msg => {
				if (msg.Model.ThingId.HasValue)
				{
					// ReSharper disable once PossibleInvalidOperationException
					Context.Handle(msg, item => DeviceService.PairDevices(item.UserName, item.Model.ActionToggle, item.Model.ThingId.Value));
				}
				else
				{
					Context.Handle(msg, item => DeviceService.PairDevices(item.UserName, item.Model.ActionToggle));
				}
				
				});
			Receive<ControllerCommand>(msg => {
				Context.Handle(msg, item => DeviceService.ControllerCommand(item.UserName, item.ThingId, item.Command, item.Arg));
				});
			Receive<CancelDeviceCommand>(msg => {
				Context.Handle(msg, item => DeviceService.CancelDeviceCommand(item.UserName, item.ThingId));
				});
			Receive<SetDeviceValue>(msg => {
				Context.Handle(msg, item => DeviceService.SetDeviceValue(item.UserName, item.ItemId, item.Value));
				});
			Receive<HardResetIoTController>(msg => {
				Context.Handle(msg, item => DeviceService.HardResetIoTController(item.UserName, item.ThingId));
				});
            Receive<SoftRestartIoTController>(msg => {
                Context.Handle(msg, item => DeviceService.SoftRestartIoTController(item.UserName, item.ThingId));
            });
            Receive<HealZwaveNetwork>(msg => {
                Context.Handle(msg, item => DeviceService.HealZwaveNetwork(item.UserName, item.ThingId));
            });
            Receive<ForceDeviceUpdate>(msg => {
                Context.Handle(msg, item => DeviceService.ForceDeviceUpdate(item.UserName, item.ThingId));
            });
            Receive<GetDeviceTypeNames>(msg => {
				Context.Handle(msg, item => DeviceService.GetDeviceTypeNames());
				});
			Receive<GetValueTypeNames>(msg => {
				Context.Handle(msg, item => DeviceService.GetValueTypeNames());
				});
			Receive<GetGenreNames>(msg => {
				Context.Handle(msg, item => DeviceService.GetGenreNames());
				});
			Receive<GetItemCategoryNames>(msg => {
				Context.Handle(msg, item => DeviceService.GetItemCategoryNames());
				});
			Receive<StartStopAudioPlugin>(msg => {
				Context.Handle(msg, item => DeviceService.StartStopAudioPlugin(item.UserName, item.ThingId, item.ActionToggle));
				});
			Receive<AudioData>(msg => {
				Context.Handle(msg, item => DeviceService.SendAudioData(item.UserName, item.ThingId, item.Data ));
				});
        }
    }
}