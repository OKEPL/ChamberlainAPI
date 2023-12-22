using System.Collections.Generic;

namespace Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Settings
{
    public class SettingsModel
    {
        public Dictionary<string, string> SettingsDictionary { get; }

        public SettingsModel(Dictionary<string, string> settingsDictionary)
        {
            SettingsDictionary = settingsDictionary;
        }

        public SettingsModel()
        {

        }

    }
}
