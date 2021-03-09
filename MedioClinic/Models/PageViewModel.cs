using CMS.Base;
using Kentico.Content.Web.Mvc;

namespace MedioClinic.Models
{
    /// <summary>
    /// Base class 
    /// </summary>
    public class PageViewModel
    {
        public IPageMetadata Metadata { get; set; } = new MedioClinic.Models.PageMetadata();

        public UserMessage UserMessage { get; set; } = new UserMessage();

        public static PageViewModel GetPageViewModel(
            IPageMetadata pageMetadata,
            string? message = default,
            bool displayMessage = true, 
            bool displayAsRaw = default, 
            MessageType messageType = MessageType.Info) =>
            new PageViewModel()
            {
                Metadata = pageMetadata,
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw,
                    Display = displayMessage
                }
            };
    }

    public class PageViewModel<TViewModel> : PageViewModel
    {
        public TViewModel Data { get; set; } = default!;

        public static PageViewModel<TViewModel> GetPageViewModel(
            TViewModel data,
            IPageMetadata pageMetadata,
            string? message = default,
            bool displayMessage = true, 
            bool displayAsRaw = default, 
            MessageType messageType = MessageType.Info) =>
            new PageViewModel<TViewModel>()
            {
                Metadata = pageMetadata,
                UserMessage = new UserMessage
                {
                    Message = message,
                    MessageType = messageType,
                    DisplayAsRaw = displayAsRaw,
                    Display = displayMessage
                },
                Data = data
            };
    }
}
