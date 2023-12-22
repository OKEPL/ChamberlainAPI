using System;
using System.Linq;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts.Camera;
using Chamberlain.Common.Content.DataContracts.ZWave;
using Chamberlain.Common.Contracts.Enums;
using Chamberlain.Common.Domotica;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Extensions;
using Serilog;

namespace Chamberlain.AppServer.Api.Hubs.Helpers
{
    public class MockItemCreationHelper
    {
        public static void AddMockItem(Entities context, Thing thing, string itemType, MockSpecialItemType mockType, IoTDeviceItemGenre valueGenre, IoTDeviceItemValueType valueType, bool isReadonly)
        {
            if (thing.Items.All(i => XmlSerialization.FromString<CameraMockItemSettings>(i.Settings, true)?.MockItemType != mockType))
            {
                var itemToAdd = new Item
                {
                    Type = itemType,
                    NativeName = Enum.GetName(typeof(MockSpecialItemType), mockType),
                    CustomName = Enum.GetName(typeof(MockSpecialItemType), mockType),
                    NativeKey = $"{thing.NativeKey}_{Enum.GetName(typeof(MockSpecialItemType), mockType)}",
                    Settings = XmlSerialization.ToString(new CameraMockItemSettings
                    {
                        MockItemType = mockType,
                        IsValueReadOnly = isReadonly,
                        ValueGenre = (int)valueGenre,
                        ValueType = (int)valueType
                    }),
                    ThingId = thing.Id,
                    LastSeen = DateTime.UtcNow
                };

                context.Items.Add(itemToAdd);
                context.SaveChanges();
            }
        }

        public static void AddZWaveMockItem(Entities context, Thing thing, ZWaveMockItem mock, string nativeKey)
        {
            Log.Debug($"Adding zwave mock item with native key {nativeKey} type:{mock.MockItemType}");
            var realItem = thing.Items?.FirstOrDefault(x => x.GetItemNativeKeyModel().CommandClass == mock.CommandClassId && x.GetItemNativeKeyModel().CommandClassIndex == mock.CommandClassIndex);
            var realItemNativeKey = realItem?.GetItemNativeKeyModel();
            var itemToAdd = new Item
            {
                Type = ItemTypes.ZwaveMock,
                NativeName = mock.ItemName,
                CustomName = mock.ItemName,
                NativeKey = nativeKey,
                Settings = XmlSerialization.ToString(new ZWaveMockItemSettings
                {
                    MockItemType = mock.MockItemType,
                    IsValueReadOnly = mock.IsValueReadOnly,
                    ValueGenre = (int)mock.ValueGenre,
                    ValueType = (int)mock.ValueType,
                    RealItemCommandClass = realItemNativeKey?.CommandClass,
                    RealItemValueIndex = realItemNativeKey?.CommandClassIndex,
                    RealItemId = realItem?.Id,
                    ValueUnits = mock.ValueUnits,
                    ListValueTypeItems = mock.ListValueTypeItems
                }),
                ThingId = thing.Id,
                LastSeen = DateTime.UtcNow,
				NativeValue = mock.RealToMockValueMapper?.DefaultValue            };

            context.Items.Add(itemToAdd);
            context.SaveChanges();
        }
    }
}
