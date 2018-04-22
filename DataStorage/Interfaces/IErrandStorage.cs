using DataStorage.DataObjects;
using Optional;
using System.Collections.Generic;

namespace DataStorage.Interfaces
{
    /// <summary>
    /// Errand creation, editing and viewing
    /// </summary>
    public interface IErrandStorage
    {
        IEnumerable<Errand> GetAll();
        Option<Errand> GetSingle(long id);
    }
}
