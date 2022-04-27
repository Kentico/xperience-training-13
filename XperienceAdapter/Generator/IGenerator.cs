using CMS.Base;

using Common;

namespace XperienceAdapter.Generator
{
    public interface IGenerator : IService
    {
        void GenerateContacts(string path);

        void GenerateFormData(string path, string formCodename, ITreeNode treeNode);

        void GenerateActivities();

        void GenerateContactGroup();

        void GenerateConversions();
    }
}
