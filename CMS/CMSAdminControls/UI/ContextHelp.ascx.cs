using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_ContextHelp : CMSUserControl
{
    /// <summary>
    /// Page load event handling.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeVersion();

        ScriptHelper.RegisterModule(Page, "CMS/ContextHelp", new
        {
            wrapperId = ClientID,
            toolbarId = pnlToolbar.ClientID,
            helpTopicsMenuItemId = helpTopics.ClientID,
            searchMenuItemId = search.ClientID,
            searchUrlPattern = DocumentationHelper.GetDocumentationSearchUrlPattern(),
            descriptionMenuItemId = description.ClientID
        });

        ScriptHelper.RegisterModule(Page, "CMS/Phoenix", new
        {
            labelId = lblVersion.ClientID
        });
    }


    /// <summary>
    /// Initializes the version label.
    /// </summary>
    private void InitializeVersion()
    {
        string version = "v";

        if (SystemContext.DevelopmentMode)
        {
            version += CMSVersion.GetVersion(true, true, true, false, true);
        }
        else
        {
            if (ValidationHelper.GetInteger(CMSVersion.HotfixVersion, 0) > 0)
            {
                version += CMSVersion.GetVersion(true, true, true, true);
            }
            else
            {
                version += CMSVersion.MainVersion;
            }
        }
        lblVersion.Text = version;
    }
}
