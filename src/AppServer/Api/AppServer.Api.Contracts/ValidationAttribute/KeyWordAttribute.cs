namespace Chamberlain.AppServer.Api.Contracts.ValidationAttribute
{
    #region

    using System;

    #endregion

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class KeyWordAttribute : Attribute
    {
        public KeyWordAttribute(string keyword)
        {
            this.KeyWord = keyword;
        }

        public string KeyWord { get; }
    }
}