namespace Chamberlain.AppServer.Api.Contracts.Commands.Modes
{
    #region

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;

    #endregion
    
    public class AddMode : HasUserName
    {
        public AddMode(string userName, ModePostModel model)
            : base(userName)
        {
            this.Model = model;
        }

        public ModePostModel Model { get; }
    }
    
    public class DeleteMode : HasUserName
    {
        public DeleteMode(string userName, long modeId)
            : base(userName)
        {
            this.ModeId = modeId;
        }

        public long ModeId { get; }
    }

    public class GetMode : HasUserName
    {
        public GetMode(string userName, long modeId)
            : base(userName)
        {
            this.ModeId = modeId;
        }

        public long ModeId { get; }
    }

    public class GetModes : HasUserName
    {
        public GetModes(string userName)
            : base(userName)
        {
        }
    }
    
    public class UpdateModeName : HasUserName
    {
        public UpdateModeName(string userName, long modeId, string name)
            : base(userName)
        {
            this.ModeId = modeId;
            this.Name = name;
        }

        public long ModeId { get; }

        public string Name { get; }
    }

    public class UpdateModeColor : HasUserName
    {
        public UpdateModeColor(string userName, long modeId, string color)
            : base(userName)
        {
            this.ModeId = modeId;
            this.Color = color;
        }

        public string Color { get; }

        public long ModeId { get; }
    }

    public class UpdateMode : HasUserName
    {
        public UpdateMode(string userName, ModeModel model)
            : base(userName)
        {
            this.Model = model;
        }

        public ModeModel Model { get; }
    }
}