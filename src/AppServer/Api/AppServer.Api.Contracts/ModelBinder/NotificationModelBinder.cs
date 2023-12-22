namespace Chamberlain.AppServer.Api.Contracts.ModelBinder
{
    using System.Text;
    using System.Threading.Tasks;

    using Chamberlain.AppServer.Api.Contracts.JsonConverter;
    using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class NotificationModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ActionContext.HttpContext.Request.ContentLength == null
                || bindingContext.ActionContext.HttpContext.Request.ContentLength == 0)
            {
                bindingContext.ModelState.AddModelError(
                    "emptyModel",
                    "Couldn't find body in model. Can't properly bind");
                return Task.CompletedTask;
            }

            var leng = (int)bindingContext.ActionContext.HttpContext.Request.ContentLength;
            var buffer = new byte[leng];
            var value = bindingContext.ActionContext.HttpContext.Request.Body.ReadAsync(buffer, 0, leng);
            value.Wait();

            var bodyJson = JObject.Parse(Encoding.UTF8.GetString(buffer)).ToString();

            var model = JsonConvert.DeserializeObject<UpdateAccountNotificationsModel>(
                bodyJson,
                new JsonNotificationConverter());
            bindingContext.Result = ModelBindingResult.Success(model);

            if (bindingContext.Result == ModelBindingResult.Failed())
            {
                bindingContext.ModelState.AddModelError(
                    "IncorrectModel",
                    "Couldn't bind to proper BaseNotificationModel class");
            }

            return Task.CompletedTask;
        }
    }
}