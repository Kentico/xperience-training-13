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
        /// <param name="allowOptIn"></param>
        Task<NewsletterSubscriptionResult<NewsletterSubscriptionResultState>> SubscribeToNewsletterAsync(NewsletterSubscriptionModel model, CancellationToken cancellationToken, bool allowOptIn);

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

        /// <summary>
        /// Gets newsletters with a boolean flag signaling if the contact is subscribed.
        /// </summary>
        /// <returns>Newsletters marked with subscription flags.</returns>
        Task<List<NewsletterPreferenceModel>> GetNewslettersForContact();

        /// <summary>
        /// Unsubscribes a single newsletter in the scope of a bulk operation.
        /// </summary>
        /// <param name="model">Subscription model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A result object with two possible states: unsubscribed and newsletter not found.</returns>
        Task<NewsletterSubscriptionResult<NewsletterUnsubscriptionResultState>> BulkUnsubscribeAsync(NewsletterSubscriptionModel model, CancellationToken cancellationToken);
    }
}
