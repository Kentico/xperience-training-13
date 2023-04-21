﻿using System.ComponentModel.DataAnnotations;

namespace XperienceAdapter.Models
{
    public class NewsletterSubscriptionConfirmationModel
    {
        /// <summary>
        /// Hash used to identify the subscription and for protection against forged confirmation requests.
        /// </summary>
        [Required]
        public string? SubscriptionHash
        {
            get;
            set;
        }

        /// <summary>
        /// The date and time of the original subscription. Used to detect expired confirmation requests.
        /// </summary>
        public string? DateTime
        {
            get;
            set;
        }
    }
}
