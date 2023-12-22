using AppServer.SettingsProvider;
using Chamberlain.Common.Settings;
using StructureMap;

namespace Chamberlain.AppServer.Api.Endpoint
{   /// <summary>
    /// Make Register Aplication Endpoint
    /// </summary>
    public class ApiEndpointRegistry : Registry
    {
        /// <summary>
        /// TODO
        /// </summary>
        public ApiEndpointRegistry()
        {
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
                scanner.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            For<ISettingsProvider>().Use<SettingsProvider>().Singleton();
        }
    }
}