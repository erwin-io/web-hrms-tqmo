using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace HRMS.Web.Models
{
    [HubName("SignalRHubWeb")]
    public class SignalRHubWeb : Hub
    {
        [HubMethodName("NotifyClients")]
        public static void BroadcastDataToAllClients()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRHubWeb>();

            // the update client method will update the connected client about any recent changes in the server data
            context.Clients.All.UpdateConversationMessage();
        }
    }
}