namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    #endregion

    public static class NotificationTypes
    {
        public static IEnumerable<Type> Types { get; } = Assembly.GetAssembly(typeof(BaseNotificationModel)).GetTypes()
            .Where(
                myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseNotificationModel)));
    }
}