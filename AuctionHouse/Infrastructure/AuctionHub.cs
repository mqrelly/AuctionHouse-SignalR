using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionHouse.DTO;
using AuctionHouse.Model;
using Microsoft.AspNet.SignalR;

namespace AuctionHouse.Infrastructure
{
    public class AuctionHub : Hub
    {
        private IConnections _connections;
        private IAuctions _auctions;

        public AuctionHub(IConnections connections, IAuctions auctions)
        {
            _connections = connections;
            _auctions = auctions;
        }

        #region Connection life-time handling

        public override Task OnConnected()
        {
            var profileName = Context.QueryString["profileName"];

            _connections.Register(Context.ConnectionId, profileName);
            System.Diagnostics.Debug.WriteLine(string.Format("Connection established: ProfileName={0}, ConnectionId={1}", profileName, Context.ConnectionId));

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _connections.Remove(Context.ConnectionId);
            System.Diagnostics.Debug.WriteLine(string.Format("Connection closed: ConnectionId={0}", Context.ConnectionId));

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Connection reestablished: ConnectionId={0}", Context.ConnectionId));

            return base.OnReconnected();
        }

        #endregion

        #region Public RPC

        public IEnumerable<DTO.AuctionItem> GetAuctions()
        {
            return _auctions.GetAll().Select(DtoFromModel);
        }

        public void CreateAuction(AuctionStartItem startItem)
        {
            try
            {
                var profileName = _connections.GetProfileName(Context.ConnectionId);
                var item = _auctions.Create(profileName, startItem);

                Clients.All.auctionUpdated(DtoFromModel(item));
            }
            catch (Exception ex)
            {
                Clients.Caller.errorMessage(ex.Message);
            }
        }

        public void CancelAuction(string auctionId)
        {
            try
            {
                var profileName = _connections.GetProfileName(Context.ConnectionId);
                var item = _auctions.Cancel(profileName, auctionId);

                Clients.All.auctionCanceled(new AuctionClosed
                {
                    Id = auctionId,
                    ItemName = item.Name
                });
            }
            catch (Exception ex)
            {
                Clients.Caller.errorMessage(ex.Message);
            }
        }

        public void CloseAuction(string auctionId)
        {
            try
            {
                var profileName = _connections.GetProfileName(Context.ConnectionId);
                var item = _auctions.Close(profileName, auctionId);
                var winnerConnectionId = _connections.GetConnectionId(item.LastBidderName);

                Clients.All.auctionClosed(new AuctionClosed
                {
                    Id = auctionId,
                    ItemName = item.Name,
                    WinnerConnectionId = winnerConnectionId,
                    WinnerName = item.LastBidderName
                });
            }
            catch (Exception ex)
            {
                Clients.Caller.errorMessage(ex.Message);
            }
        }

        public void Bid(string auctionId, int amount)
        {
            try
            {
                var bidderName = _connections.GetProfileName(Context.ConnectionId);
                var item = _auctions.Bid(bidderName, auctionId, amount);

                Clients.All.auctionUpdated(DtoFromModel(item));
            }
            catch (Exception ex)
            {
                Clients.Caller.errorMessage(ex.Message);
            }
        }

        public void ResetProfileName(string profileName)
        {
            _connections.Remove(Context.ConnectionId);
            _connections.Register(Context.ConnectionId, profileName);
        }

        #endregion

        #region Helpers

        private DTO.AuctionItem DtoFromModel(Model.AuctionItem item)
        {
            return new DTO.AuctionItem
            {
                Id = item.Id,
                CurrentPrise = item.CurrentPrise,
                Name = item.Name,
                LastBidderConnectionId = _connections.GetConnectionId(item.LastBidderName),
                OwnerConnectionId = _connections.GetConnectionId(item.OwnerName)
            };
        }

        #endregion
    }
}