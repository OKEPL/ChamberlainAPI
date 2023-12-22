using System;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    #region
    
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.Commands.Customers;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Authentication;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers;
    using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Account;
    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.Common.Akka;

    using global::AppServer.Api.Endpoint.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <inheritdoc />
    /// <summary>
    /// User controller class.
    /// </summary>
    [Produces("application/json")]
    [Route("user")]
    public class UserController : ChamberlainBaseController
    {
        /// <summary>
        /// Adds single email notification for user.
        /// </summary>
        /// <param name="model">
        /// E-mail notification model.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost("addEmail")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEmail([FromBody] EmailModel model)
        {
            await SystemActors.CustomerEmailActor.Execute(new AddEmail(User.Identity.Name, model.Email, model.Alerts, model.Newsletters));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Update emails by deleting existing one and replacing them with new if given in request.
        /// </summary>
        /// <param name="emailModelList">
        /// List of email's models.
        /// </param>
        /// <returns>
        /// Returns 204 if updated successfully.
        /// </returns>
        [HttpPut("updateEmails")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEmails([FromBody] List<EmailModel> emailModelList)
        {
            await SystemActors.CustomerEmailActor.Execute(new UpdateEmails(User.Identity.Name, emailModelList));
            return NoContent();
        }

        /// <summary>
        /// Adds single firebasepush notification for user.
        /// </summary>
        /// <param name="model">
        /// FirebasePush notification model.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost]
        [Route("firebasePush")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddFirebasePush([FromBody] FirebasePushModel model)
        {
            await SystemActors.CustomerPushActor.Execute(new AddFirebasePush(User.Identity.Name, model.Firebase, model.Label, model.Alerts));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Adds single Ifttt notification for user.
        /// </summary>
        /// <param name="model">
        /// Ifttt notification model.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost("ifttt")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddIfttt([FromBody] IftttModel model)
        {
            await SystemActors.CustomerIftttActor.Execute(new AddIfttt(User.Identity.Name, model.Ifttt, model.Label, model.Alerts));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Update ifttt's keys by deleting existing one and replacing them with new if given in request.
        /// </summary>
        /// <param name="iftttModelList">
        /// List of ifttt's models.
        /// </param>
        /// <returns>
        /// Returns 204 if updated successfully.
        /// </returns>
        [HttpPut("updateIfttts")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateIfttts([FromBody] List<IftttModel> iftttModelList)
        {
            await SystemActors.CustomerIftttActor.Execute(new UpdateIfttts(User.Identity.Name, iftttModelList));
            return NoContent();
        }

        /// <summary>
        /// Adds single Sms notification for user.
        /// </summary>
        /// <param name="model">
        /// Sms notification model.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost("addSms")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddSms([FromBody] SmsModel model)
        {
            await SystemActors.CustomerSmsActor.Execute(new AddSms(User.Identity.Name, model.PhoneNumber, model.Label, model.Alerts, model.Voip));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Update smses by deleting existing one and replacing them with new if given in request.
        /// </summary>
        /// <param name="smsModelList">
        /// List of sms's models.
        /// </param>
        /// <returns>
        /// Returns 204 if updated successfully.
        /// </returns>
        [HttpPut("updateSmses")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateSmses([FromBody] List<SmsModel> smsModelList)
        {
            await SystemActors.CustomerSmsActor.Execute(new UpdateSmses(User.Identity.Name, smsModelList));
            return NoContent();
        }

        /// <summary>
        /// Update security phones by deleting existing one and replacing them with new if given in request.
        /// </summary>
        /// <param name="securityPhoneModelList">
        /// List of security phone's models.
        /// </param>
        /// <returns>
        /// Returns 204 if updated successfully.
        /// </returns>
        [HttpPut("updateSecurity")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateSecurity([FromBody] List<SecurityPhoneModel> securityPhoneModelList)
        {
            await SystemActors.CustomerSmsActor.Execute(new UpdateSecurityPhones(User.Identity.Name, securityPhoneModelList));
            return NoContent();
        }

        /// <summary>
        /// Changes timezone of user.
        /// </summary>
        /// <param name="model">
        /// Model with new timezone Id. Using ids from: https://msdn.microsoft.com/en-us/library/gg154758.aspx
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("updateTimezone")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ChangeTimezone([FromBody] ChangeTimezoneModel model)
        {
            await SystemActors.CustomerActor.Execute(new ChangeTimezone(User.Identity.Name, model.Timezone));
            return NoContent();
        }

        /// <summary>
        /// Changes user mode to new mode.
        /// </summary>
        /// <param name="modeId">
        /// ModeId of new mode for user.</param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("changeMode/{modeId:long}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ChangeUserMode(long modeId)
        {
            await SystemActors.CustomerActor.Execute(new ChangeUserMode(User.Identity.Name, modeId));
            return NoContent();
        }

        /// <summary>
        /// Change users future to new.
        /// </summary>
        /// <param name="featureId">
        /// FeatureId of new future.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("changeFeature/{featureId:long}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> ChangeUserSubscription(long featureId)
        {
            await SystemActors.CustomerActor.Execute(new ChangeUserSubscription(User.Identity.Name, featureId));
            return NoContent();
        }

        /// <summary>
        /// Creates new NestAuthentication session for user.
        /// </summary>
        /// <param name="redirectTo">
        /// Http address of redirection.
        /// </param>
        /// <returns>
        /// Returns session hash.
        /// </returns>
        [HttpPost("createNestAuthenticationSession")]
        [ProducesResponseType(typeof(NestAuthenticationSessionModel), 200)]
        public async Task<IActionResult> CreateNestAuthenticationSession(string redirectTo)
        {
            var result = await SystemActors.CustomerActor.Execute<CreateNestAuthenticationSession, NestAuthenticationSessionModel>(
                new CreateNestAuthenticationSession(User.Identity.Name, redirectTo));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Deletes email notification from user.
        /// </summary>
        /// <param name="email">
        /// Email address used for notification which should be deleted.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("delEmail/{email}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEmail(string email)
        {
            await SystemActors.CustomerEmailActor.Execute(new DeleteEmail(User.Identity.Name, email));
            return NoContent();
        }

        /// <summary>
        /// Deletes firebase push notification from user.
        /// </summary>
        /// <param name="name">
        /// Firebase push notification used for notification which should be deleted.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("firebasePush/{name}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteFirebasePush(string name)
        {
            await SystemActors.CustomerPushActor.Execute(new DeleteFirebasePush(User.Identity.Name, name));
            return NoContent();
        }

        /// <summary>
        /// Deletes ifttt notification from user.
        /// </summary>
        /// <param name="name">
        /// Ifttt notification used for notification which should be deleted.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("delIfttt/{name}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteIfttt(string name)
        {
            await SystemActors.CustomerIftttActor.Execute(new DeleteIfttt(User.Identity.Name, name));
            return NoContent();
        }

        /// <summary>
        /// Deletes sms notification from user.
        /// </summary>
        /// <param name="sms">
        /// Phone number used for notification which should be deleted.</param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("delSms/{sms}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteSms(string sms)
        {
            await SystemActors.CustomerSmsActor.Execute(new DeleteSms(User.Identity.Name, sms));
            return NoContent();
        }

        /// <summary>
        /// Discards active nest connection of user.
        /// </summary>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("discardNestConnection")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DiscardNestConnection()
        {
            await SystemActors.CustomerActor.Execute(new DiscardNestConnection(User.Identity.Name));
            return NoContent();
        }

        /// <summary>
        /// Gets account data of user.
        /// </summary>
        /// <returns>
        /// Information about current user from databae.
        /// </returns>
        [HttpGet("getAccountData")]
        [ProducesResponseType(typeof(AccountDataModel), 200)]
        public async Task<IActionResult> GetAccountData()
        {
            var result = await SystemActors.CustomerActor.Execute<GetAccountData, AccountDataModel>(new GetAccountData(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all email notifications from user.
        /// </summary>
        /// <returns>
        /// List of email notifications.
        /// </returns>
        [HttpGet("getEmails")]
        [ProducesResponseType(typeof(List<EmailModel>), 200)]
        public async Task<IActionResult> GetEmails()
        {
            var result = await SystemActors.CustomerEmailActor.Execute<GetEmails, List<EmailModel>>(new GetEmails(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets all firebase push notifications from user.
        /// </summary>
        /// <returns>
        /// List of firebase push notifications.
        /// </returns>
        [HttpGet("firebasePush")]
        [ProducesResponseType(typeof(List<FirebasePushModel>), 200)]
        public async Task<IActionResult> GetFirebasePush()
        {
            var result = await SystemActors.CustomerPushActor.Execute<GetFirebasePush, List<FirebasePushModel>>(new GetFirebasePush(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets list of possible timezones.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("timezones")]
        public async Task<IActionResult> GetTimezones()
        {
            var result = await SystemActors.CustomerActor.Execute<GetTimezones, List<TimeZoneInfoModel>>(new GetTimezones());
            return new ObjectResult(result);
        }

        /// <summary>
        ///  Gets all ifttt notifications from user.
        /// </summary>
        /// <returns>
        /// List of ifttt notifications.
        /// </returns>
        [HttpGet("ifttt")]
        [ProducesResponseType(typeof(List<IftttModel>), 200)]
        public async Task<IActionResult> GetIfttt()
        {
            var result = await SystemActors.CustomerIftttActor.Execute<GetIfttt, List<IftttModel>>(new GetIfttt(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Get nest token
        /// </summary>
        /// <returns>
        /// Redirect nest token
        /// </returns>
        [AllowAnonymous]
        [HttpGet("getNestToken")]
        [ProducesResponseType(303)]
        public async Task<IActionResult> GetNestToken(string state, string code)
        {
            var result = await SystemActors.CustomerActor.Execute<GetNestToken, NestRedirectionModel>(new GetNestToken(state, code));
            return Redirect(result.RedirectUri.AbsoluteUri);
        }

        /// <summary>
        /// Gets all sms notifications from user.
        /// </summary>
        /// <returns>
        /// List of sms notifications.
        /// </returns>
        [HttpGet("getSms")]
        [ProducesResponseType(typeof(List<SmsModel>), 200)]
        public async Task<IActionResult> GetSms()
        {
            var result = await SystemActors.CustomerSmsActor.Execute<GetSms, List<SmsModel>>(new GetSms(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets information abot current user.
        /// </summary>
        /// <returns>
        /// Information about current logged in user.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserModel), 200)]
        public async Task<IActionResult> GetUser()
        {
            var result = await SystemActors.CustomerActor.Execute<GetUser, UserModel>(new GetUser(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets information about user features.
        /// </summary>
        /// <returns>
        /// Information about currents user subscription.
        /// </returns>
        [HttpGet("feature")]
        [ProducesResponseType(typeof(UserSubscriptionModel), 200)]
        public async Task<IActionResult> GetUserSubscription()
        {
            var result = await SystemActors.CustomerActor.Execute<GetUserSubscription, UserSubscriptionModel>(new GetUserSubscription(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Update notifications by deleting existing one and replacing them with new if given in request.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// <para>
        ///     PUT /user/updateNotifications
        ///     {
        ///         "Notifications":
        ///             [{
        ///                 "PhoneNumber" : "1111",
        ///                 "Label" : "Home",
        ///                 "Alerts" : "true",
        ///                 "Voip" : "false"
        ///             },
        ///             {
        ///                 "PhoneNumber" : "2222",
        ///                 "Label" : "Work",
        ///                 "Alerts" : "false",
        ///                 "Voip" : "true"
        ///             },
        ///             {
        ///                 "Email" : "asd@asd.com",
        ///                 "Label" : "Home",
        ///                 "Alerts" : "false",
        ///                 "Newsletters" : "true"
        ///             },
        ///             {
        ///                 "Ifttt" : "2222",
        ///                 "Label" : "HomeIfttt",
        ///                 "Alerts" : "false"
        ///             },
        ///             {
        ///                 "Firebase" : "Firebase",
        ///                 "Label" : "myFirebase",
        ///                 "Alerts" : "true"
        ///             }]
        ///     }
        /// </para>
        /// </remarks>
        /// <param name="data">
        /// Model with list of different notifications as one list. If any type of notification is given in this model, already existing one will be deleted from database.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [Obsolete]
        [HttpPut("updateNotifications")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateNotifications([FromBody] UpdateAccountNotificationsModel data)
        {
            await SystemActors.CustomerActor.Execute(new UpdateNotifications(User.Identity.Name, data));
            return NoContent();
        }

        /// <summary>
        /// Update recording brackets time of user with new given one.
        /// </summary>
        /// <param name="model">
        /// Model with pre and post recording times. Pre have to be lower then Post else model is not valid.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("updateRecordingBracketsTime")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRecordingBracketsTime([FromBody] UpdateRecordingBracketsTimeModel model)
        {
            await SystemActors.CustomerActor.Execute(new UpdateRecordingBracketsTime(User.Identity.Name, model.PreRecTime.Value, model.PostRecTime.Value));
            return NoContent();
        }

        /// <summary>
        /// Gets address of user.
        /// </summary>
        /// <returns>
        /// Information about user's address from database.
        /// </returns>
        [HttpGet("getAddress")]
        [ProducesResponseType(typeof(CustomerAddressModel), 200)]
        public async Task<IActionResult> GetAddress()
        {
            var result = await SystemActors.CustomerActor.Execute<GetAddress, CustomerAddressModel>(new GetAddress(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Assign address to user.
        /// </summary>
        /// <param name="model">
        /// Customer Address model.
        /// </param>
        /// <returns>
        /// Returns 201 if created successfully.
        /// </returns>
        [HttpPost("assignAddress")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AssignAddress([FromBody] CustomerAddressModel model)
        {
            await SystemActors.CustomerActor.Execute(new AssignAddress(User.Identity.Name, model));
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Update of user address.
        /// </summary>
        /// <param name="model">
        /// Customer Address Model.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpPut("updateAddress")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateAddress([FromBody] CustomerAddressModel model)
        {
            await SystemActors.CustomerActor.Execute(new UpdateAddress(User.Identity.Name, model));
            return NoContent();
        }
    }
}