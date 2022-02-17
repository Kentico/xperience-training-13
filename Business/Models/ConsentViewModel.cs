using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models
{
    public class ConsentViewModel
    {
        public string? CodeName { get; set; }

        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? ShortText { get; set; }

        public string? FullText { get; set; }

        public bool Agreed { get; set; }

        public int? CookieLevel { get; set; }
    }
}
