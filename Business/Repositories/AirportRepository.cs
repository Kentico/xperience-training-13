using System;
using System.Collections.Generic;
using System.Text;

using Core;
using Business.Models;
using System.Threading.Tasks;
using CMS.CustomTables;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.CustomTables.Types.MedioClinic;
using System.Threading;
using System.Linq;

namespace Business.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private const string Code = "AirportIataCode";

        private const string Name = "AirportName";

        public IEnumerable<Airport> GetAll() => GetAllAsync().GetAwaiter().GetResult();

        public async Task<IEnumerable<Airport>> GetAllAsync(CancellationToken? cancellationToken = default) =>
            await GetResultAsync(null, cancellationToken);

        public async Task<IEnumerable<Airport>> GetBySearchPhraseAsync(string? searchPhrase = default,
            CancellationToken? cancellationToken = default) =>
            !string.IsNullOrEmpty(searchPhrase)
            ? await GetResultAsync(filter => filter
                .WhereContains(Name, searchPhrase),
                cancellationToken)
            : Enumerable.Empty<Airport>();

        private async Task<IEnumerable<Airport>> GetResultAsync(
            Func<ObjectQuery<AirportsItem>, ObjectQuery<AirportsItem>>? filter,
            CancellationToken? cancellationToken)
        {
            var query = CustomTableItemProvider.GetItems<AirportsItem>();

            if (filter != null)
            {
                query = filter(query);
            }

            return (await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken))
                .Select(item => MapDtoProperties(item));
        }

        private static Airport MapDtoProperties(AirportsItem item) => new Airport
        {
            AirportIataCode = item.AirportIataCode,
            AirportName = item.AirportName
        };
    }
}
