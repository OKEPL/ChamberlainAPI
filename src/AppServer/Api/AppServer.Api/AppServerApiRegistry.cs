using AppServer.SettingsProvider;
using Chamberlain.Common.Settings;
using PredefinedRulesManager.SceneRules;
using PredefinedRulesManager.Scenes;
using StructureMap;

namespace Chamberlain.AppServer.Api
{
    public class AppServerRegistry : Registry
    {
        public AppServerRegistry()
        {
           For<ISettingsProvider>().Use<SettingsProvider>().Singleton();
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
                scanner.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            For<IPredefinedSceneRulesManagerPlugin>().Use<PredefinedSceneRulesManagerPlugin>().Singleton();
        }
    }
}
