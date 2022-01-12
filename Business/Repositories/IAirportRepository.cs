using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Business.Models;

using Common;

namespace Business.Repositories
{
    public interface IAirportRepository : IRepository<Airport>
    {
        Task<IEnumerable<Airport>> GetBySearchPhraseAsync(
            string? searchPhrase = default, CancellationToken? cancellationToken = default);
    }
}
