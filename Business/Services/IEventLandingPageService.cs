using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Business.Models;

using CMS.DocumentEngine;

using Common;

using Kentico.Content.Web.Mvc;

namespace Business.Services
{
    public interface IEventLandingPageService : IService
    {
        /// <summary>
        /// Gets the date when an event happens.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<DateTime?> GetEventDateAsync(TreeNode page, CancellationToken cancellationToken);
    }
}
