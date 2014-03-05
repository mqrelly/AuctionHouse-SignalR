using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AuctionHouse.DTO;

namespace AuctionHouse.Model
{
    public class InMemoryAuctions : IAuctions
    {
        private static object _lock = new object();
        private static Dictionary<string, AuctionItem> _auctions = new Dictionary<string, AuctionItem>();
        private static int _lastId;

        public AuctionItem Create(string connectionId, AuctionStartItem startItem)
        {
            lock (_lock)
            {
                _lastId += 1;

                var item = new AuctionItem
                {
                    Id = _lastId.ToString(CultureInfo.InvariantCulture),
                    OwnerName = connectionId,
                    Name = startItem.Name,
                    CurrentPrise = startItem.Price
                };

                _auctions[item.Id] = item;

                return item;
            }
        }

        public AuctionItem Cancel(string ownerName, string auctionId)
        {
            lock (_lock)
            {
                AuctionItem item;
                if (!_auctions.TryGetValue(auctionId, out item))
                    throw new Exception("Auction not found.");

                if (item.OwnerName != ownerName)
                    throw new Exception("Only the owner of the auction can cancel it.");

                _auctions.Remove(auctionId);

                return item;
            }
        }

        public AuctionItem Close(string ownerName, string auctionId)
        {
            lock (_lock)
            {
                AuctionItem item;
                if (!_auctions.TryGetValue(auctionId, out item))
                    throw new Exception("Auction not found.");

                if (item.OwnerName != ownerName)
                    throw new Exception("Only the owner of the auction can close it.");

                _auctions.Remove(auctionId);

                return item;
            }
        }

        public AuctionItem Bid(string name, string auctionId, int amount)
        {
            lock (_lock)
            {
                AuctionItem item;
                if (!_auctions.TryGetValue(auctionId, out item))
                    throw new Exception("Auction not found.");

                if (item.CurrentPrise < amount)
                {
                    item.CurrentPrise = amount;
                    item.LastBidderName = name;
                }

                return item;
            }
        }

        public IEnumerable<AuctionItem> GetAll()
        {
            IEnumerable<AuctionItem> snapshot;
            lock (_lock)
            {
                snapshot = _auctions.Values.ToList();
            }

            return snapshot;
        }
    }
}