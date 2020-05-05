using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[Title("content.newabtesttitle")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_NewPage : CMSContentPage
{
    #region "Page methods and events"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Check module permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.ABTest", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "read", "A/B testing"));
        }

        // Check UI Permissions
        if ((MembershipContext.AuthenticatedUser == null) || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New.ABTestVariant")))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "New.ABTestVariant");
        }

        // Header actions
        CurrentMaster.HeaderActions.AddAction(new SaveAction
        {
            ResourceName = "CMS.ABTest",
            Permission = "Manage"
        });
        CurrentMaster.HeaderActions.AddAction(new SaveAction
        {
            ResourceName = "CMS.ABTest",
            Permission = "Manage",
            RegisterShortcutScript = false,
            Text = GetString("editmenu.iconsaveandanother"),
            Tooltip = GetString("EditMenu.SaveAndAnother"),
            CommandArgument = "true"
        });
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Register title script
        ScriptHelper.RegisterEditScript(Page, false);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisabledModule.ParentPanel = pnlDisabled;

        ScriptHelper.RegisterLoader(Page);

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetInteger("saved", 0) == 1)
            {
                ShowChangesSaved();
            }
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        // Check the permissions
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.abtest", "Manage"))
        {
            ShowError(GetString("om.abtest.nomanagepermission"));
            return;
        }

        bool createAnother = ValidationHelper.GetBoolean(e.CommandArgument, false);

        // Create document
        int newNodeID = ucNewPage.Save();
        if (newNodeID != 0)
        {
            // Refresh tree
            string script = null;
            if (createAnother)
            {
                int parentNodeID = QueryHelper.GetInteger("parentnodeid", 0);
                if (parentNodeID != 0)
                {
                    string param = (QueryHelper.GetInteger("saved", 0) == 0) ? "&saved=1" : String.Empty;

                    script = ScriptHelper.GetScript("RefreshTree(" + newNodeID + ", " + parentNodeID + ");CreateAnotherWithParam('" + param + "');");
                }
            }
            else
            {
                script = ScriptHelper.GetScript("RefreshTree(" + newNodeID + ", " + newNodeID + ");SelectNode(" + newNodeID + ");");
            }
            ScriptHelper.RegisterClientScriptBlock(Page, typeof (string), "Refresh", script);
        }
    }

    #endregion
}
