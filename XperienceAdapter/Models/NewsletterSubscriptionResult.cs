using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XperienceAdapter.Models
{
    public class NewsletterSubscriptionResult<TResultState>
        where TResultState : Enum
    {
        public bool Success { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public TResultState ResultState { get; set; } = default!;
    }

    public class NewsletterSubscriptionResult<TResultState, TData> : NewsletterSubscriptionResult<TResultState>
        where TResultState : Enum
    {
        public TData Data { get; set; } = default!;
    }
}
