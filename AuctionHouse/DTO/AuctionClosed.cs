using Newtonsoft.Json;

namespace AuctionHouse.DTO
{
    public class AuctionClosed
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string ItemName { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "winnerId")]
        public string WinnerConnectionId { get; set; }

        [JsonProperty(PropertyName = "winnerName")]
        public string WinnerName { get; set; }
    }
}