using CMS.Base;

namespace MedioClinic.Models
{
    /// <summary>
    /// Base class 
    /// </summary>
    public class PageViewModel
    {
        public PageMetadata? Metadata { get; set; }

        public UserMessage? UserMessage { get; set; }

        public static PageViewModel GetPageViewModel(
            string title,
            ISiteService siteService,
            string? message = default,
            bool displayAsRaw = false,
            MessageType messageType = MessageType.Info) =>
            new PageViewModel()
            {
                Metadata = GetPageMetadata(title, siteService),
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw
                }
            };

        protected static PageMetadata GetPageMetadata(string title, ISiteService siteService) =>
            new PageMetadata()
            {
                Title = title,
                CompanyName = siteService.CurrentSite.DisplayName
            };

        public class PageMetadata
        {
            public string? Title { get; set; }
            public string? CompanyName { get; set; }
        }
    }

    public class PageViewModel<TViewModel> : PageViewModel where TViewModel : class
    {
        public TViewModel? Data { get; set; }

        public static PageViewModel<TViewModel> GetPageViewModel(
            TViewModel data,
            string title,
            ISiteService siteService,
            string? message = default,
            bool displayAsRaw = false,
            MessageType messageType = MessageType.Info) =>
            new PageViewModel<TViewModel>()
            {
                Metadata = GetPageMetadata(title, siteService),
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw
                },
                Data = data
            };
    }
}
