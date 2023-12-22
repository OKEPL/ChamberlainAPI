using Common.StaticMethods.StaticMethods;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region
    using System;
    using System.Threading.Tasks;
    using Contracts.Commands.Customers;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account;
    using Helpers;
    using Models;
    using Chamberlain.Common.Akka;
    using Chamberlain.Common.Settings;
    using Chamberlain.ExternalServices.Email;

    using global::AppServer.Api.Endpoint.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    
    #endregion

    /// <inheritdoc />
    /// <summary>
    /// Account Controller class
    /// </summary>
    [Route("account")]
    public class AccountController : ChamberlainBaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Main constructor.
        /// </summary>
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        /// <summary>
        /// Activate users account.
        /// </summary>
        /// <param name="model">
        /// Model for user activation. Takes token from registration e-mail and username.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded or didn't find user.
        /// </returns>
        [AllowAnonymous]
        [HttpPut("activate")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Activate([FromBody] ActivateAccountModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NoContent();

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (!result.Succeeded)
                return BadRequest(new { errors = ErrorMapHelper.MapIdentityErrors(result) });

            return NoContent();
        }

        /// <summary>
        /// Change password of user.
        /// </summary>
        /// <param name="model">
        /// Model with both old and new password.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded or didn't find user.
        /// </returns>
        [HttpPut]
        [Route("password/change")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NoContent();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { errors = ErrorMapHelper.MapMultipleIdentityErrors(result.Errors) });

            return NoContent();
        }

        /// <summary>
        /// Change pin of user.
        /// </summary>
        /// <param name="model">
        /// Model with both old and new pin for change.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [ProducesResponseType(204)]
        [HttpPut("pin/change")]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return NoContent();

            if (user.PinHash != StaticMethods.HashPassword(model.OldPin.ToString()))
            {
                return BadRequest(
                    new
                        {
                            errors = ErrorMapHelper.MapSingleErrorMessage(
                                nameof(ChangePinModel.OldPin),
                                "Old pin does not match")
                        });
            }

            user.PinHash = StaticMethods.HashPassword(model.NewPin.ToString());
            user.PinDenail = null;
            user.Attemps = 0;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        /// <summary>
        /// Creates username using information given in model.
        /// </summary>
        /// <param name="model">
        /// Model with username information to create account.
        /// </param>
        /// <returns>
        /// 201 if created successfully.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create([FromBody] AddUserModel model)
        {
            var identityUser = _userManager.FindByNameAsync(model.UserName);
            var customer = await SystemActors.CustomerActor.Execute<GetUser, UserModel>(new GetUser(model.UserName));
            identityUser.Wait();
            if (customer != null || identityUser.Result != null)
                return BadRequest(new { error = (dynamic)"Customer already exists in database." });

            var user = new ApplicationUser
                           {
                               UserName = model.UserName,
                               Email = model.Email,
                               PinHash = StaticMethods.HashPassword(
                                   _configuration["Account:DefaultPin"]),
                           };

            var result = await _userManager.CreateAsync(user, model.Password);
            string token;

            if (result.Succeeded)
            {
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                SendCustomerRegisterEmail(model, token);
                await SystemActors.CustomerActor.Execute(new AddUser(model.UserName, model.Email, model.Password));
            }
            else
                return BadRequest(new { errors = ErrorMapHelper.MapMultipleIdentityErrors(result.Errors) });

            return Created(string.Empty, _configuration["Enviroment"] == "Test" ? token : null);
        }

        /// <summary>
        /// Resets password for username.
        /// </summary>
        /// <param name="name">
        /// Username to reset.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("password/reset/{name}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ResetPassword(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return NoContent();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var mailBody = _emailSender.GetEmailTemplate("CustomerRestorePasswordEmail");
            mailBody = mailBody.Replace("{link}", CachedSettings.Get("CustomerPasswordMailLink", "http://localhost/Account/Login?method=restore&amp;Token="));
            mailBody = mailBody.Replace("{token}", token);
            mailBody = mailBody.Replace("{userName}", user.UserName);

            _emailSender.Send(user.Email, "Domotica change password email.", mailBody);

            if (_configuration["Enviroment"] == "Test")
                return Created(string.Empty, token);

            return NoContent();
        }

        /// <summary>
        /// Verify if given Pin is accurate.
        /// </summary>
        /// <param name="model">
        /// Pin to verify, 4 numbers.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("pin/verify")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> VerifyPin([FromBody] VerifyPinModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user.PinDenail.HasValue && user.Attemps >= CachedSettings.Get("PinMaxFailure", 3))
                return StatusCode(403, "PIN lockout. Relogin to unlock PIN.");

            if (user.PinHash != StaticMethods.HashPassword(model.Pin))
            {
                user.Attemps += 1;
                user.PinDenail = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                return StatusCode(403, "Wrong PIN.");
            }

            user.Attemps = 0;
            user.PinDenail = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        private void SendCustomerRegisterEmail(AddUserModel model, string token)
        {
            var mailBody = _emailSender.GetEmailTemplate("CustomerRegisterEmail");
            mailBody = mailBody.Replace("{link}", 
                CachedSettings.Get("CustomerRegisterMailLink", "http://localhost/Account/Login?method=activate&amp;Token="));
            mailBody = mailBody.Replace("{token}", token);
            mailBody = mailBody.Replace("{userName}", model.UserName);
            _emailSender.Send(model.Email, "Domotica registration email.", mailBody);
        }
    }
}