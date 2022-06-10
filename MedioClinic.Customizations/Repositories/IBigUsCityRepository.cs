using MedioClinic.Customizations.Models;

using Common;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedioClinic.Customizations.Repositories
{
    public interface IBigUsCityRepository : IRepository<BigUsCity>
    {
        IEnumerable<BigUsCity> GetByNameAndStateCode(string cityName, string stateCode);

        Task<IEnumerable<BigUsCity>> GetByNameAndStateCodeAsync(
            string cityName, string stateCode, CancellationToken cancellationToken);
    }
}
