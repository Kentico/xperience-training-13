using TinyCsvParser.Mapping;

namespace XperienceAdapter.Generator
{
    internal class FormDataMapping : CsvMapping<FormData>
    {
        public FormDataMapping() : base()
        {
            MapProperty(0, m => m.CompanyName);
            MapProperty(1, m => m.Type);
            MapProperty(2, m => m.ReasonsToJoin);
            MapProperty(3, m => m.StartingDate);
            MapProperty(4, m => m.PhotoOrMap);
            MapProperty(5, m => m.FirstName);
            MapProperty(6, m => m.LastName);
            MapProperty(7, m => m.EmailInput);
            MapProperty(8, m => m.ConsentAgreementForms);
            MapProperty(9, m => m.ConsentAgreementFiles);
        }
    }
}
