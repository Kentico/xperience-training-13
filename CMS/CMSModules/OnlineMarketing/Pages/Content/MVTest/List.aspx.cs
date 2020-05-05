using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


[Security(Resource = "CMS.MVTest", UIElements = "MVTestListing")]
[UIElement("CMS.MVTest", "MVTestListing")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_List : CMSMVTestPage
{
    /// <summary>
    /// If true, the items are edited in dialog
    /// </summary>
    private bool EditInDialog
    {
        get
        {
            return listElem.Grid.EditInDialog;
        }
        set
        {
            listElem.Grid.EditInDialog = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditInDialog = QueryHelper.GetBoolean("editindialog", false);
    }
    

    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisabledModule.ParentPanel = pnlDisabled;

        InitHeaderActions();
        InitTitle();
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MVTest", "New"))
        {
            string url = UIContextHelper.GetElementUrl("CMS.MVTest", "New", EditInDialog);

            // Get the alias path of the current node
            if (Node != null)
            {
                // Set NodeID in order to check the access to the document
                listElem.NodeID = Node.NodeID;
                listElem.AliasPath = Node.NodeAliasPath;

                url = URLHelper.AddParameterToUrl(url, "NodeID", Node.NodeID.ToString());
                url = URLHelper.AddParameterToUrl(url, "AliasPath", Node.NodeAliasPath);
            }

            // Set header action
            var action = new HeaderAction
            {
                ResourceName = "CMS.MVTest",
                Permission = "Manage",
                Text = GetString("mvtest.new"),
                RedirectUrl = ResolveUrl(url),
                OpenInDialog = EditInDialog
            };

            CurrentMaster.HeaderActions.AddAction(action);
        }
    }


    /// <summary>
    /// Sets title if not in content.
    /// </summary>
    private void InitTitle()
    {
        if (NodeID <= 0)
        {
            SetTitle(GetString("analytics_codename.mvtests"));
        }
    }
}