using Newtonsoft.Json;

namespace AuctionHouse.DTO
{
    public class AuctionItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        public string OwnerConnectionId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "currentPrice")]
        public int CurrentPrise { get; set; }

        [JsonProperty(PropertyName = "lastBidderId")]
        public string LastBidderConnectionId { get; set; }
    }
}