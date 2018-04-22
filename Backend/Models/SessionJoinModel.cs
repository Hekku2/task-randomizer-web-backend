using System;

namespace Backend.Models
{
    public class SessionJoinModel
    {
        public Guid SessionId { get; set; }
        public string PlayerName { get; set; }
    }
}
