using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;

public partial class CMSModules_PortalEngine_Controls_Layout_PlaceholderMenu : CMSAbstractPortalUserControl
{
    protected PageInfo pi = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Prepare script to display versions dialog
        StringBuilder script = new StringBuilder();
        script.Append(
            @"
function ShowVersionsDialog(objectType, objectId, objectName) {
  modalDialog('", ResolveUrl("~/CMSModules/Objects/Dialogs/ObjectVersionDialog.aspx"), @"' + '?objecttype=' + objectType + '&objectid=' + objectId + '&objectname=' + objectName,'VersionsDialog','800','600');
}"
            );

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowVersionDialog", ScriptHelper.GetScript(script.ToString()));

        PageTemplateInfo pti = null;

        if (mPagePlaceholder != null)
        {
            pi = mPagePlaceholder.PageInfo;
        }

        if (pi != null)
        {
            pti = pi.UsedPageTemplateInfo;
        }

        bool documentExists = ((pi != null) && (pi.DocumentID > 0));

        if ((mPagePlaceholder != null) && (mPagePlaceholder.ViewMode == ViewModeEnum.DesignDisabled))
        {
            // Hide edit layout and edit template if design mode is disabled
            iLayout.Visible = false;
            iTemplate.Visible = false;
        }
        else
        {
            if ((mPagePlaceholder != null) && (mPagePlaceholder.LayoutTemplate == null))
            {
                // Edit layout
                iLayout.Text = ResHelper.GetString("PlaceholderMenu.IconLayout", culture);
                iLayout.Attributes.Add("onclick", "EditLayout();");

                if ((pti != null) && (pti.LayoutID > 0))
                {
                    LayoutInfo li = LayoutInfoProvider.GetLayoutInfo(pti.LayoutID);

                    // Display layout versions sub-menu
                    if ((li != null) && ObjectVersionManager.DisplayVersionsTab(li))
                    {
                        menuLayout.Visible = true;
                        iLayout.Text = ResHelper.GetString("PlaceholderMenu.IconLayoutMore", culture);
                        lblSharedLayoutVersions.Text = ResHelper.GetString("PlaceholderMenu.SharedLayoutVersions", culture);
                        pnlSharedLayout.Attributes.Add("onclick", GetVersionsDialog(li.TypeInfo.ObjectType, li.LayoutId));
                    }
                }
            }
            else
            {
                iLayout.Visible = false;
            }

            if (documentExists)
            {
                // Template properties
                iTemplate.Text = ResHelper.GetString("PlaceholderMenu.IconTemplate", culture);

                int templateID = (pti != null) ? pti.PageTemplateId : 0;
                String aliasPath = (pi != null) ? pi.NodeAliasPath : "";

                String url = ApplicationUrlHelper.GetElementDialogUrl("cms.design", "PageTemplate.EditPageTemplate",templateID, String.Format("aliaspath={0}", aliasPath));
                iTemplate.Attributes.Add("onclick", String.Format("modalDialog('{0}', 'edittemplate', '95%', '95%');", url));

                if (pti != null)
                {
                    // Display template versions sub-menu
                    if (ObjectVersionManager.DisplayVersionsTab(pti))
                    {
                        menuTemplate.Visible = true;
                        iTemplate.Text = ResHelper.GetString("PlaceholderMenu.IconTemplateMore", culture);
                        lblTemplateVersions.Text = ResHelper.GetString("PlaceholderMenu.TemplateVersions", culture);
                        pnlTemplateVersions.Attributes.Add("onclick", GetVersionsDialog(pti.TypeInfo.ObjectType, pti.PageTemplateId));
                    }
                }
            }
        }

        if (pti != null)
        {
            if (documentExists)
            {
                if (!SynchronizationHelper.UseCheckinCheckout || DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser))
                {
                    iClone.Text = ResHelper.GetString("PlaceholderMenu.IconClone", culture);
                    iClone.OnClientClick = "CloneTemplate(GetContextMenuParameter('pagePlaceholderMenu'));";
                }
            }
        }

        iRefresh.Text = ResHelper.GetString("PlaceholderMenu.IconRefresh", culture);
        iRefresh.Attributes.Add("onclick", "RefreshPage();");
    }


    /// <summary>
    /// Gets javascript to open modal versions dialog.
    /// </summary>
    /// <param name="objType">Object type</param>
    /// <param name="objId">ID of the object</param>
    private string GetVersionsDialog(string objType, int objId)
    {
        string url = ResolveUrl("~/CMSModules/Objects/Dialogs/ObjectVersionDialog.aspx?objecttype=" + objType + "&objectid=" + objId);
        return "modalDialog('" + URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)) + "','VersionsDialog','900','750');return false;";
    }
}
