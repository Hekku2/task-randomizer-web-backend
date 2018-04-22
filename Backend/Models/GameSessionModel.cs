using System;

namespace Backend.Models
{
    public class GameSessionModel
    {
        public Guid Id { get; set; }
        public string GameName { get; set; }
        public ErrandModel[] Errands { get; set; }
        public string[] Players { get; set; }
    }
}
