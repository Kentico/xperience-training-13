using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS;
using CMS.CustomTables;
using CMS.CustomTables.Types.MedioClinic;
using CMS.DataEngine;
using CMS.Helpers;

using MedioClinic.Customizations.Models;
using MedioClinic.Customizations.Repositories;

[assembly: RegisterImplementation(typeof(IBigUsCityRepository), typeof(BigUsCityRepository))]

namespace MedioClinic.Customizations.Repositories
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

        public ObjectQuery<BigUsCitiesItem> GetAllItems() =>
            GetQuery(null);

        public IEnumerable<BigUsCity> GetByNameAndStateCode(string cityName, string stateCode)
        {
            return !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(stateCode)
                ? GetResult(filter => filter
                    .WhereEquals(nameof(BigUsCitiesItem.CityName), cityName)
                    .WhereEquals(nameof(BigUsCitiesItem.StateCode), stateCode),
                    $"{nameof(BigUsCityRepository)}|{nameof(GetByNameAndStateCode)}|{cityName}|{stateCode}",
                    CacheDependencies)
                : Enumerable.Empty<BigUsCity>();
        }

        public async Task<IEnumerable<BigUsCity>> GetByNameAndStateCodeAsync(string cityName, string stateCode, CancellationToken cancellationToken)
        {
            return !string.IsNullOrEmpty(cityName) && !string.IsNullOrEmpty(stateCode)
                ? await GetResultAsync(filter => filter
                    .WhereEquals(nameof(BigUsCitiesItem.CityName), cityName)
                    .WhereEquals(nameof(BigUsCitiesItem.StateCode), stateCode),
                    cancellationToken,
                    $"{nameof(BigUsCityRepository)}|{nameof(GetByNameAndStateCodeAsync)}|{cityName}|{stateCode}",
                    CacheDependencies)
                : Enumerable.Empty<BigUsCity>();
        }

        private async Task<IEnumerable<BigUsCity>> GetResultAsync(
            Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>> filter,
            CancellationToken? cancellationToken,
            string cacheKey,
            params string[] cacheDependencies)
        {
            var query = GetQuery(filter);
            var cacheSettings = GetCacheSettings(cacheKey);

            return (await _progressiveCache.LoadAsync(async modifiedCacheSettings =>
            {
                modifiedCacheSettings.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);

                return await query.GetEnumerableTypedResultAsync(cancellationToken: cancellationToken);
            },
                cacheSettings))
                .Select(item => MapDtoProperties(item));
        }

        private IEnumerable<BigUsCity> GetResult(
            Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>> filter,
            string cacheKey,
            params string[] cacheDependencies)
        {
            var query = GetQuery(filter);
            var cacheSettings = GetCacheSettings(cacheKey);

            return _progressiveCache.Load(modifiedCacheSettings =>
                {
                    modifiedCacheSettings.CacheDependency = CacheHelper.GetCacheDependency(cacheDependencies);

                    return query.GetEnumerableTypedResult();
                },
                cacheSettings)
                .Select(item => MapDtoProperties(item));
        }

        private static ObjectQuery<BigUsCitiesItem> GetQuery(Func<ObjectQuery<BigUsCitiesItem>, ObjectQuery<BigUsCitiesItem>> filter)
        {
            // The repository cannot guarantee that other objects (e.g. ContactInfo) belong to just american cities.
            // It is the responsibility of the client code to also check, whether the contact's city belongs to a US state, not some other.
            var query = CustomTableItemProvider.GetItems<BigUsCitiesItem>();

            if (filter != null)
            {
                query = filter(query);
            }

            return query;
        }

        private static CacheSettings GetCacheSettings(string cacheKey) =>
            new CacheSettings(TimeSpan.FromMinutes(10).TotalMinutes, cacheKey);

        private static BigUsCity MapDtoProperties(BigUsCitiesItem item) => new BigUsCity
        {
            CityId = item.ItemID,
            CityName = item.CityName,
            StateCode = item.StateCode
        };
    }
}
