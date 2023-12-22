using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.StructureMapContent;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PredefinedRulesManager.Interfaces;

namespace Chamberlain.AppServer.Api.Helpers
{
    public static class HelperMethods
    {
        public static string RetrieveNestAccessToken(string authorizationCode)
        {
            var url =
                $"https://api.home.nest.com/oauth2/access_token?code={authorizationCode}&client_id={"aa52b4bd-c1b3-4df3-95b4-cbb96b7b378a"}&client_secret={"MsqC8PnQLLE9G0BTTTnVAWuW5"}&grant_type=authorization_code";

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.PostAsync(url, content: null).Result)
                {
                    var token = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                    return ((dynamic) token).access_token;
                }
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            var md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static void DeleteThing(string userName, long thingId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Things).ThenInclude(x => x.Items).First(x => x.Username.Equals(userName));
                var thing = customer.Things.First(x => x.Id == thingId);
                
                // Delete Items
                foreach (var item in thing.Items)
                {
                    RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.StopHandlingItemMessageType, customer.Id,
                        item.Id, null, string.Empty));
                }
                
                context.Things.Remove(thing);
                context.SaveChanges();
            }
        }
    }
}