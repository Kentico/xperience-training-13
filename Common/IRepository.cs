using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Provides data from a data source in the form of a DTO object.
    /// </summary>
    /// <typeparam name="TDto">Type of the DTO.</typeparam>
    public interface IRepository<TDto>
    {
        /// <summary>
        /// Gets all items from the source.
        /// </summary>
        /// <returns>All items.</returns>
        IEnumerable<TDto> GetAll();

        /// <summary>
        /// Gets all items from the source asynchronously.
        /// </summary>
        /// <returns>All items.</returns>
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken? cancellationToken = default);
    }
}
