namespace AuctionHouse.Model
{
    public class AuctionItem
    {
        public string Id { get; set; }

        public string OwnerName { get; set; }

        public string Name { get; set; }

        public int CurrentPrise { get; set; }

        public string LastBidderName { get; set; }
    }
}