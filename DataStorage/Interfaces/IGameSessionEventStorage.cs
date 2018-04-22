using DataStorage.DataObjects.Events;
using System;
using System.Collections.Generic;

namespace DataStorage.Interfaces
{
    public interface IGameSessionEventStorage
    {
        /// <summary>
        /// Returns game events in order, where first event in list is first event in game
        /// </summary>
        /// <param name="sessionId">ID of session</param>
        /// <returns>Events of session</returns>
        List<Event> GetEvents(Guid sessionId);
    }
}
