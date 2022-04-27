using System;

namespace XperienceAdapter.Generator
{
    internal class FormData
    {
        public string? CompanyName { get; set; }

        public string? Type { get; set; }

        public string? ReasonsToJoin { get; set; }

        public DateTime? StartingDate { get; set; }

        public Guid? PhotoOrMap { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailInput { get; set; }

        public Guid? ConsentAgreementForms { get; set; }

        public Guid? ConsentAgreementFiles { get; set; }
    }
}
