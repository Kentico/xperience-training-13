namespace XperienceAdapter.Models
{
    public enum NewsletterSubscriptionResultState
    {
        OtherErrors,
        NewsletterNotFound,
        ContactNotFound,
        ContactNotSet,
        NotSubscribed,
        Subscribed
    }

    public enum NewsletterSubscriptionConfirmationResultState
    {
        OtherErrors,
        InvalidDateTime,
        SubscriptionNotFound,
        TimeExceeded,
        NotConfirmed,
        Confirmed
    }

    public enum NewsletterUnsubscriptionResultState
    {
        OtherErrors,
        InvalidHash,
        IssueNotFound,
        AlreadyUnsubscribedFromAll,
        NewsletterNotFound,
        AlreadyUnsubscribed,
        Unsubscribed
    }
}
