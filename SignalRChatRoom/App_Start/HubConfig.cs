using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

namespace SignalRChatRoom
{
    public static class HubConfig
    {
        public static void RegisterHubs()
        {
            // Register the default hubs route: ~/signalr
            RouteTable.Routes.MapHubs();
        }
    }
}