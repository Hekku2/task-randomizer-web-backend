using System;
using System.Collections.Generic;

namespace DataStorage.DataObjects
{
    public class GameSession
    {
        public Guid Id { get; set; }

        public string GameName { get; set; }

        public List<Errand> Errands { get; set; }

        public List<string> Players { get; set; }
    }
}
