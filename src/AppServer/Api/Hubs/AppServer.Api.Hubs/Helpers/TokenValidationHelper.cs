using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Chamberlain.Common.Extensions;

namespace Chamberlain.AppServer.Api.Hubs.Helpers
{
    public static class TokenValidationHelper
    {
        private static readonly JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();
        private static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("da39a3ee5e6b4b0d3255bfef95601890afd80709"));

        private static readonly TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey,

            ValidateIssuer = false,
            ValidIssuer = "dev.solomio.com",

            ValidateAudience = false,
            ValidAudience = "dev.solomio.com",

            ValidateLifetime = false
        };

        public static bool Validate(string token, out string userName)
        {
            userName = string.Empty;
            try
            {
                var principal = Handler.ValidateToken(token, TokenValidationParameters, out var validToken);
                if (!(validToken is JwtSecurityToken))
                    return false;

                userName = principal.Identity.Name;
                return true;
            }
            catch (Exception e)
            {
                Log.Warning($"Failed to validate the user: {e.GetDetails()}");
                return false;
            }
        }
    }
}