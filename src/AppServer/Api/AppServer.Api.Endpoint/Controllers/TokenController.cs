namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region

    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account;
    using Chamberlain.AppServer.Api.Endpoint.Models;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Token controller class.
    /// </summary>
    public class TokenController : ChamberlainBaseController
    {
        private readonly IConfiguration configuration;

        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Token controller default constructor.
        /// </summary>
        public TokenController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        /// <summary>
        /// Generate pernament token for user.
        /// </summary>
        /// <param name="model">
        /// Login model with user name and password.
        /// </param>
        /// <returns>
        /// Token as string if successful</returns>
        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenModel), 200)]
        public async Task<IActionResult> GenerateToken([FromBody] LoginModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return this.BadRequest(new { error = "Could not create token" });
            }

            var result = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return this.BadRequest(new { error = "Could not create token" });
            }

            if (user.Attemps != 0)
            {
                user.Attemps = 0;
                user.PinDenail = null;
                await this.userManager.UpdateAsync(user);
            }

            var options = new IdentityOptions();
            var utc0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var issueTime = DateTime.Now;
            var iat = (long)issueTime.Subtract(utc0).TotalSeconds;

            var claims = new[]
                             {
                                 new Claim(JwtRegisteredClaimNames.Aud, model.PolicyType.ToString()),
                                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                 new Claim(
                                     JwtRegisteredClaimNames.Iat,
                                     iat.ToString(),
                                     ClaimValueTypes.Integer64),
                                 new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id),
                                 new Claim(options.ClaimsIdentity.UserNameClaimType, user.UserName)
                             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                this.configuration["Tokens:Issuer"],
                this.configuration["Tokens:Issuer"],
                claims,
                signingCredentials: creds);

            var tokenModel = new TokenModel { Token = new JwtSecurityTokenHandler().WriteToken(token) };
            return this.Ok(tokenModel);
        }
    }
}