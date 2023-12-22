namespace Chamberlain.AppServer.Api.Contracts
{
    public class StaticExpressions
    {
        public const string NoGaps = "^[^ ]*$";

        public const string NonNegativeNumbers = "^[1-9][0-9]*$";

        public const string PhoneNumber = "^[0-9()-]*$";

        public const string PostalCode = "^[0-9- ]*$";

        public const string String = "^[a-zA-Z]*$";

        public const string StringWithGaps = "^[a-zA-Z- ]*$";

        public const string StringWithNumbers = "^[a-zA-Z0-9]*$";

        public const string StringWithNumbersAndGaps = "^[a-zA-Z0-9- ]*$";

        public const string UserNameConvention = "^[a-zA-Z0-9]+([._-][a-zA-Z0-9]+)*$";
    }
}