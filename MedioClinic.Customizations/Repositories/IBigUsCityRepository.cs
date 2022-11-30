using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.CustomTables.Types.MedioClinic;
using CMS.DataEngine;

using Common;
using MedioClinic.Customizations.Models;

namespace MedioClinic.Customizations.Repositories
{
    public interface IBigUsCityRepository : IRepository<BigUsCity>
    {
        /// <summary>
        /// Gets a big US city by its name and state code.
        /// </summary>
        /// <param name="cityName">City name.</param>
        /// <param name="stateCode">State code.</param>
        /// <returns>Sequence of big US cities.</returns>
        IEnumerable<BigUsCity> GetByNameAndStateCode(string cityName, string stateCode);

        /// <summary>
        /// Gets a big US city by its name and state code.
        /// </summary>
        /// <param name="cityName">City name.</param>
        /// <param name="stateCode">State code.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Sequence of big US cities.</returns>
        Task<IEnumerable<BigUsCity>> GetByNameAndStateCodeAsync(string cityName, string stateCode, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a query for all the <see cref="BigUsCitiesItem"/> custom table objects.
        /// </summary>
        /// <returns>A query for the custom table items.</returns>
        ObjectQuery<BigUsCitiesItem> GetAllItems();
    }
}
