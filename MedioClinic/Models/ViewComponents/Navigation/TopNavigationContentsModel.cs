using Abstractions;

namespace MedioClinic.Models.ViewComponents.Navigation
{
    public class TopNavigationContentsModel
    {
        public string? MainId { get; set; }
        
        public string? CultureSwitchId { get; set; }

        public string? ListClass { get; set; }

        public NavigationItem? NavigationItem { get; set; }

        public bool DisplaySecondLevel { get; set; }
    }
}
