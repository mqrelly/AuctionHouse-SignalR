using System;
using System.Collections.Generic;

namespace AuctionHouse.Infrastructure
{
    class InMemoryConnections : IConnections
    {
        private static object _lock = new object();
        private static Dictionary<string, string> _connsToNames = new Dictionary<string, string>();
        private static Dictionary<string, string> _namesToConns = new Dictionary<string, string>();

        public string GetProfileName(string connectionId)
        {
            if (connectionId == null)
                return null;

            lock (_lock)
            {
                string name;
                return _connsToNames.TryGetValue(connectionId, out name) ? name : null;
            }
        }

        public void Register(string connectionId, string profileName)
        {
            lock (_lock)
            {
                if(_namesToConns.ContainsKey(profileName))
                    throw new Exception("Profile name is already taken: " + profileName);

                _connsToNames[connectionId] = profileName;
                _namesToConns[profileName] = connectionId;
            }
        }

        public void Remove(string connectionId)
        {
            lock (_lock)
            {
                string profileName;
                if (!_connsToNames.TryGetValue(connectionId, out profileName))
                    return;

                _connsToNames.Remove(connectionId);
                _namesToConns.Remove(profileName);
            }
        }


        public string GetConnectionId(string profileName)
        {
            if (profileName == null)
                return null;

            lock (_lock)
            {
                string connectionId;
                return _namesToConns.TryGetValue(profileName, out connectionId) ? connectionId : null;
            }
        }
    }
}