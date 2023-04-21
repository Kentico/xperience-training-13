using CMS.Base;
using CMS.DocumentEngine;

using Common;

using DocumentFormat.OpenXml.Office2010.ExcelAc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace XperienceAdapter.Generator
{
    public interface IGenerator : IService
    {
        void GenerateContacts(string path);

        void GenerateFormData(string path, string formCodename, ITreeNode treeNode);

        void GenerateActivities();

        void GenerateContactGroup();

        void GenerateAbTestConversions(TreeNode page, string requestDomain);

        List<string> GenerateNewsletterSubscribers(string path);

        void GenerateNewsletterOpensAndLinkClicks(string path, List<string> subscribers);
    }
}
