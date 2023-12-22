namespace Chamberlain.AppServer.Api.Contracts.Commands.Customers
{
    #region

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers;

    #endregion

    public class AddUser
    {
        public AddUser(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Name { get; }
        public string Password { get; }
    }

    public class ChangePassword : HasUserName
    {
        public ChangePassword(string userName, string oldPassword, string newPassword)
            : base(userName)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public string NewPassword { get; }
        public string OldPassword { get; }
    }

    public class ChangeTimezone : HasUserName
    {
        public ChangeTimezone(string userName, int timezone)
            : base(userName)
        {
            Timezone = timezone;
        }

        public int Timezone { get; }
    }

    public class ChangeUserMode : HasUserName
    {
        public ChangeUserMode(string userName, long modeId)
            : base(userName)
        {
            ModeId = modeId;
        }

        public long ModeId { get; }
    }

    public class ChangeUserSubscription : HasUserName
    {
        public ChangeUserSubscription(string userName, long featureId)
            : base(userName)
        {
            FeatureId = featureId;
        }

        public long FeatureId { get; }
    }

    public class GetAccountData : HasUserName
    {
        public GetAccountData(string userName)
            : base(userName)
        {
        }
    }

    public class GetUser : HasUserName
    {
        public GetUser(string userName)
            : base(userName)
        {
        }
    }

    public class GetUserSubscription : HasUserName
    {
        public GetUserSubscription(string userName)
            : base(userName)
        {
        }
    }

    public class UpdateRecordingBracketsTime : HasUserName
    {
        public UpdateRecordingBracketsTime(string userName, long preRecTime, long postRecTime)
            : base(userName)
        {
            PreRecTime = preRecTime;
            PostRecTime = postRecTime;
        }

        public long PostRecTime { get; }
        public long PreRecTime { get; }
    }

    public class UpdateNotifications : HasUserName
    {
        public UpdateNotifications(string userName, UpdateAccountNotificationsModel data)
            : base(userName)
        {
            Data = data;
        }

        public UpdateAccountNotificationsModel Data { get; }
    }

    public class UpdateSettingsNew : HasUserName
    {
        public UpdateSettingsNew(string userName, UpdateAccountNotificationsModel data)
            : base(userName)
        {
            Data = data;
        }

        public UpdateAccountNotificationsModel Data { get; }
    }

    public class CreateNestAuthenticationSession : HasUserName
    {
        public CreateNestAuthenticationSession(string userName, string redirectTo)
            : base(userName)
        {
            RedirectTo = redirectTo;
        }

        public string RedirectTo { get; set; }
    }

    public class GetNestToken
    {
        public GetNestToken(string state, string code)
        {
            State = state;
            Code = code;
        }

        public string Code { get; set; }
        public string State { get; set; }
    }

    public class DiscardNestConnection : HasUserName
    {
        public DiscardNestConnection(string userName)
            : base(userName)
        {
        }
    }

    public class GetAddress : HasUserName
    {
        public GetAddress(string userName)
            : base(userName)
        {
        }
    }

    public class AssignAddress : HasUserName
    {
        public AssignAddress(string userName, CustomerAddressModel data)
            : base(userName)
        {
            Data = data;
        }

        public CustomerAddressModel Data { get; }
    }

    public class UpdateAddress : HasUserName
    {
        public UpdateAddress(string userName, CustomerAddressModel data)
            : base(userName)
        {
            Data = data;
        }

        public CustomerAddressModel Data { get; }
    }

    public class GetTimezones
    {
    }
}