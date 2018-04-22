using DataStorage.DataObjects;
using Optional;
using System.Collections.Generic;

namespace DataStorage.Interfaces
{
    public interface IGameStorage
    {
        IEnumerable<Game> GetAll();
        Option<Game> GetSingle(long id);
    }
}