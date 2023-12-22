using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Rule
{
    public class DeviceItem : BaseChamberlainModel
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
    }

    public class DeviceItemDetails : DeviceItem
    {
        public string ItemType { get; set; }
        public DeviceOptions Options { get; set; }
    }

    public class DeviceOptions
    {
        public string WebControlType { get; set; }
    }

    public class NumericValueOptions : DeviceOptions
    {
        public List<NumericValueOption> ValueOptions { get; set; }
    }

    public class NumericValueOption
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class ValueWithLabelOptions : DeviceOptions
    {
        public ValueWithLabelOptions()
        {
            Values = new List<ValueWithLabel>();
        }

        public List<ValueWithLabel> Values { get; set; }
    }

    public class ValueWithLabel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}