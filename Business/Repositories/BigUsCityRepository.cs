using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.CustomTables;
using CMS.CustomTables.Types.MedioClinic;
using CMS.DataEngine;
using CMS.Helpers;

using Business.Models;

namespace Business.Repositories
{
    public class BigUsCityRepository : IBigUsCityRepository
    {
        private const string CacheDependencies = "CustomTableItem.MedioClinic.BigUsCities|All";

        private readonly IProgressiveCache _progressiveCache;

        public BigUsCityRepository(IProgressiveCache progressiveCache)
        {
            _progressiveCache = progressiveCache ?? throw new ArgumentNullException(nameof(progressiveCache));
        }

        public IEnumerable<BigUsCity> GetAll() =>
            GetResult(null,
                $"{nameof(BigUsCityRepository)}|{nameof(GetAll)}",
                CacheDependencies);

        public async Task<IEnumerable<BigUsCity>> GetAllAsync(CancellationToken? cancellationToken = null) =>
            await GetResultAsync(null, cancellationToken,
                $"{nameof(BigUsCityRepository)}|{nameof(GetAllAsync)}",
                CacheDependencies);

        public IEnumerable<BigUsCity> GetByNameAndStateCode(string cityName, string stateCode) =>
            !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(stateCode)
            ? GetResult(filter => filter
                .WhereEquals(nameof(BigUsCitiesItem.CityName), cityName)
                .WhereEquals(nameof(BigUsCitiesItem.StateCode), stateCode),
                $"{nameof(BigUsCityRepository)}|{nameof(GetByNameAndStateCode)}|{cityName}|{stateCode}",
                CacheDependencies)
            : Enumerable.Empty<BigUsCity>();

        public async Task<IEnumerable<BigUsCity>> GetByNameAndStateCodeAsync(string cityName, string stateCode, CancellationToken cancellationToken) =>
            !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(stateCode)
            ? await GetResultAsync(filter => filter
                .WhereEquals(nameof(BigUsCitiesItem.CityName), cityName)
                .WhereEquals(nameof(BigUsCitiesItem.StateCode), stateCode),
                cancellationToken,
                $"{nameof(BigUsCityRepository)}|{nameof(GetByNameAndStateCodeAsync)}|{cityName}|{stateCode}",
                CacheDependencies)
            : Enumerable.Empty<BigUsCity>();

        private async Task<IEnumerable<BigUsCity>> GetResultAsync(
            Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>>? filter,
            CancellationToken? cancellationToken,
            string cacheKey,
            params string[] cacheDependencies)
        {
            var query = GetQuery(filter);
            var cacheSettings = GetCacheSettings(cacheKey, cacheDependencies);

            return (await _progressiveCache.LoadAsync(async _ =>
                await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken),
                cacheSettings))
                .Select(item => MapDtoProperties(item));
        }

        private IEnumerable<BigUsCity> GetResult(
            Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>>? filter,
            string cacheKey,
            params string[] cacheDependencies)
        {
            var query = GetQuery(filter);
            var cacheSettings = GetCacheSettings(cacheKey, cacheDependencies);

            return _progressiveCache.Load(_ => 
                query.GetEnumerableTypedResult(),
                cacheSettings)
                .Select(item => MapDtoProperties(item));
        }

        private static ObjectQuery<BigUsCitiesItem> GetQuery(Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>>? filter)
        {
            var query = CustomTableItemProvider.GetItems<BigUsCitiesItem>();

            if (filter != null)
            {
                query = filter(query);
            }

            return query;
        }

        private static CacheSettings GetCacheSettings(string cacheKey, params string[] cacheDependencies)
        {
            var settings = new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);
            settings.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);

            return settings;
        }

        private static BigUsCity MapDtoProperties(BigUsCitiesItem item) => new BigUsCity
        {
            CityId = item.ItemID,
            CityName = item.CityName,
            StateCode = item.StateCode
        };
    }
}
