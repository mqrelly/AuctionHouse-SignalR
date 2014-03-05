using System.Collections.Generic;
using AuctionHouse.DTO;

namespace AuctionHouse.Model
{
    public interface IAuctions
    {
        IEnumerable<AuctionItem> GetAll();

        AuctionItem Create(string ownerName, AuctionStartItem startItem);

        AuctionItem Cancel(string ownerName, string auctionId);

        AuctionItem Close(string ownerName, string auctionId);

        AuctionItem Bid(string name, string auctionId, int amount);
    }
}
