namespace AuctionHouse.Infrastructure
{
    public interface IConnections
    {
        string GetProfileName(string connectionId);

        string GetConnectionId(string profileName);

        void Register(string connectionId, string profileName);

        void Remove(string connectionId);
    }
}
