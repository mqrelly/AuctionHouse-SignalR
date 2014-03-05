using AuctionHouse.Model;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AuctionHouse.Infrastructure.Startup))]

namespace AuctionHouse.Infrastructure
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(
                typeof(AuctionHub),
                () => new AuctionHub(
                    new InMemoryConnections(),
                    new InMemoryAuctions()));

            app.MapSignalR();
        }
    }
}
