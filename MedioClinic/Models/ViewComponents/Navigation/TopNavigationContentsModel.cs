using System.Collections.Generic;

using Business.Dtos;

namespace MedioClinic.Models.ViewComponents.Navigation
{
    public class TopNavigationContentsModel
    {
        public string? MainId { get; set; }
        
        public string? CultureSwitchId { get; set; }

        public string? ListClass { get; set; }

        public Dictionary<string, NavigationItem>? Navigation { get; set; }

        public bool DisplaySecondLevel { get; set; }
    }
}
