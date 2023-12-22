using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts;
using Microsoft.AspNetCore.Identity;

namespace Chamberlain.AppServer.Api.Endpoint.Helpers
{
    /// <summary>
    /// The error map helper
    /// </summary>
    public static class ErrorMapHelper
    {
        /// <summary>
        /// The first char to lower
        /// </summary>
        public static string FirstCharToLower(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// The map identity errors
        /// </summary>
        public static dynamic MapIdentityErrors(IdentityResult result)
        {
            string MapIdentityErrorToProperty(IdentityError error)
            {
                var userNameMap = new[]
                                      {
                                          "DuplicateUserName", "DuplicateName", "ExternalLoginExists",
                                          "InvalidUserName", "PropertyTooShort", "UserAlreadyHasPassword",
                                          "UserNameNotFound", "UserNotInRole", "UserAlreadyInRole"
                                      };
                if (userNameMap.Contains(error.Code))
                    return FirstCharToLower(nameof(AddUserModel.UserName));

                var emailMap = new[] { "DuplicateEmail", "InvalidEmail" };
                if (emailMap.Contains(error.Code))
                    return FirstCharToLower(nameof(AddUserModel.Email));

                var passwordMap = new[]
                                      {
                                          "PasswordMismatch", "PasswordRequireDigit", "PasswordRequireLower",
                                          "PasswordRequiresLower", "PasswordRequiresUpper",
                                          "PasswordRequireNonLetterOrDigit", "PasswordRequiresNonLetterOrDigit",
                                          "PasswordRequireUpper", "PasswordTooShort", "PasswordRequiresDigit"
                                      };
                if (passwordMap.Contains(error.Code))
                    return FirstCharToLower(nameof(AddUserModel.Password));

                var tokenMap = new[] { "InvalidToken" };
                if (tokenMap.Contains(error.Code))
                    return FirstCharToLower(nameof(ActivateAccountModel.Token));

                return error.Code;
            }

            return result.Errors.Select(error => new { Name = MapIdentityErrorToProperty(error), error.Description }).ToList();
        }

        /// <summary>
        /// The map multiple identity errors.
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static List<object> MapMultipleIdentityErrors(IEnumerable<IdentityError> results)
        {
            return results.Select(x => MapSingleErrorMessage(x.Code, x.Description)).ToList();
        }

        /// <summary>
        /// The map single message error
        /// </summary>
        public static object MapSingleErrorMessage(string name, string description)
        {
            return new { Name = FirstCharToLower(name), Description = description };
        }
    }
}