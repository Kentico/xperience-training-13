using Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XperienceAdapter.Models;

namespace XperienceAdapter.Services
{
    public interface IEmailCommunicationService : IService
    {
        /// <summary>
        /// Subscribes a contact to a newsletter.
        /// </summary>
        /// <param name="model">An upload model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A result object.</returns>
        Task<NewsletterSubscriptionResult<NewsletterSubscriptionResultState>> SubscribeToNewsletterAsync(NewsletterSubscriptionModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Confirms subscription to a newsletter (double opt-in).
        /// </summary>
        /// <param name="model">An upload model.</param>
        /// <returns>A result object.</returns>
        NewsletterSubscriptionResult<NewsletterSubscriptionConfirmationResultState> ConfirmSubscription(NewsletterSubscriptionConfirmationModel model);

        /// <summary>
        /// Unsubscribes a contact from a newsletter.
        /// </summary>
        /// <param name="model">An upload model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A result object.</returns>
        Task<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>> UnsubscribeAsync(NewsletterUnsubscriptionModel model, CancellationToken cancellationToken);
    }
}
