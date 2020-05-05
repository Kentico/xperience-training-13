using System;

using CMS.DocumentEngine;

namespace XperienceAdapter
{
    public class DbConnectivityTest
    {
        public Guid GetDocumentGuid()
        {
            var node = DocumentHelper.GetDocument(1, new TreeProvider());

            return node.DocumentGUID;
        }
    }
}
