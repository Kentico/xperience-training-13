using System;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

// Set page title
[Title("contentmenu.properties")]
public partial class CMSModules_Content_CMSDesk_Properties_Properties_Header : CMSModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Display separator
		AbstractMasterPage mPage = CurrentMaster as AbstractMasterPage;
        if (mPage != null)
        {
            mPage.DisplaySeparatorPanel = true;
        }

        // Add the document name to the properties header title
        TreeNode node = DocumentManager.Node;
        if (node != null)
        {
            string nodeName = node.GetDocumentName();
            // Get name for root document
            if (node.IsRoot())
            {
                nodeName = SiteContext.CurrentSite.DisplayName;
            }

            PageTitle.TitleText += " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"";
        }
    }

    #endregion
}