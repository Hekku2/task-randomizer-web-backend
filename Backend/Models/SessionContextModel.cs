using System;

namespace Backend.Models
{
    /// <summary>
    /// Model used when users want to perform operations as player in session
    /// </summary>
    public class SessionContextModel
    {
        public Guid SessionId { get; set; }
        public string PlayerName { get; set; }
    }
}
