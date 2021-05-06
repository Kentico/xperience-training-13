using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using Newtonsoft.Json;

namespace MedioClinic.Models
{
    public class PageTemplateProperties : IPageTemplateProperties
    {
        [JsonIgnore]
        public UserMessage? UserMessage { get; set; }
    }
}
