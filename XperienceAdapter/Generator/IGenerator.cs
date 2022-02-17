using Common;

namespace XperienceAdapter.Generator
{
    public interface IGenerator : IService
    {
        void GenerateContacts(string path);

        void GenerateActivities();

        void GeneratePersona();

        void GenerateContactGroup();

        void GenerateConversions();
    }
}
