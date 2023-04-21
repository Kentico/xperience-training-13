using System;
using System.ComponentModel.DataAnnotations;

namespace XperienceAdapter.Models
{

    public class NewsletterUnsubscriptionModel
    {
        /// <summary>
        /// The email address of the recipient who is requesting unsubscription.
        /// </summary>
        [Required]
        public string? Email
        {
            get;
            set;
        }

        /// <summary>
        /// The GUID (identifier) of the Xperience email feed related to the unsubscription request.
        /// </summary>
        [Required]
        public Guid NewsletterGuid
        {
            get;
            set;
        }

        /// <summary>
        /// The GUID (identifier) of the Xperience marketing email related to the unsubscription request.
        /// </summary>
        [Required]
        public Guid IssueGuid
        {
            get;
            set;
        }

        /// <summary>
        /// Hash for protection against forged unsubscription requests.
        /// </summary>
        [Required]
        public string? Hash
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the unsubscription request is for all marketing emails or only a specific email feed.
        /// </summary>
        public bool UnsubscribeFromAll
        {
            get;
            set;
        }
    }
}
