using Automate.Expense.Tracking.Sample.Dialog;
using Automate.Expense.Tracking.Sample.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Automate.Expense.Tracking.Sample.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
            {
                if (activity.IsComposeExtensionQuery())
                {
                    var response = MessageExtension.HandleMessageExtensionQuery(connector, activity);
                    return response != null
                        ? Request.CreateResponse<ComposeExtensionResponse>(response)
                        : new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    switch (activity.Type)
                    {
                        case ActivityTypes.Message:
                            await Conversation.SendAsync(activity, () => new RootDialog());
                            break;
                        case ActivityTypes.Invoke:
                            return HandleInvokeMessages(activity);
                    }

                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
            }
        }

        private HttpResponseMessage HandleInvokeMessages(Activity activity)
        {
            var activityValue = activity.Value.ToString();

            var reply = activity.CreateReply();
            reply.Text = "Request Approved. Expense Id: " ;

            return Request.CreateResponse(HttpStatusCode.OK, reply);
        }

    }
}
