using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Workflows_CMSPages_Comment : CMSPage
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    public string Action
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }


    /// <summary>
    /// Tree node
    /// </summary>
    protected TreeNode Node
    {
        get
        {
            return DocumentManager.Node;
        }
    }

    #endregion


    #region "Events"

    protected override void OnPreInit(EventArgs e)
    {
        EnsureDocumentManager = true;

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        // Initialize header
        InitHeader();

        // Prevent registering 'SaveChanges' script
        DocumentManager.RegisterSaveChangesScript = false;

        // Set current node
        ucComment.Node = Node;

        base.OnLoad(e);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ucComment.Visible)
        {
            btnOk.OnClientClick = "ProcessAction(" + ScriptHelper.GetString(Action) + "); return CloseDialog();";
        }
        // Dialog is not available for unauthorized user
        else
        {
            plcContent.Visible = false;
            btnOk.Visible = false;
            ShowError(GetString("doc.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        switch (Action)
        {
            case DocumentComponentEvents.PUBLISH:
                btnOk.ResourceString = "general.publish";
                break;

            case DocumentComponentEvents.APPROVE:
                btnOk.ResourceString = "general.approve";
                break;

            case DocumentComponentEvents.REJECT:
                btnOk.ResourceString = "general.reject";
                break;

            case DocumentComponentEvents.ARCHIVE:
                btnOk.ResourceString = "general.archive";
                break;

            case DocumentComponentEvents.CHECKIN:
                btnOk.ResourceString = "EditMenu.IconCheckIn";
                break;
        }

        // Set title
        string resName = HTMLHelper.HTMLEncode(Action);
        var step = DocumentManager.Step;
        if ((Action == DocumentComponentEvents.APPROVE) && (step != null) && (step.StepIsEdit))
        {
            resName += "Submit";
        }

        PageTitle.TitleText = GetString("editmenu.iconcomment" + resName);
    }

    #endregion
}