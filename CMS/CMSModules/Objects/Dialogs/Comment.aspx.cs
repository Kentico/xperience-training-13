using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Objects_Dialogs_Comment : CMSModalPage
{
    #region "Properties"

    /// <summary>
    /// Action
    /// </summary>
    protected string ActionName
    {
        get
        {
            return QueryHelper.GetString("acname", null);
        }
    }

    #endregion


    #region "Events"

    protected override void OnLoad(EventArgs e)
    {
        // Initialize header
        InitHeader();

        base.OnLoad(e);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ucComment.Visible)
        {
            SetSaveJavascript("ProcessAction(" + ScriptHelper.GetString(ActionName) + "); return CloseDialog();");
        }
        // Dialog is not available for unauthorized user
        else
        {
            plcContent.Visible = false;
            ShowError(GetString("doc.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        switch (ActionName)
        {
            case DocumentComponentEvents.CHECKIN:
                SetSaveResourceString("EditMenu.IconCheckIn");
                break;
        }

        // Set title
        string resName = HTMLHelper.HTMLEncode(ActionName);
        var step = DocumentManager.Step;
        if ((ActionName == DocumentComponentEvents.APPROVE) && (step != null) && (step.StepIsEdit))
        {
            resName += "Submit";
        }

        PageTitle.TitleText = GetString("objecteditmenu.iconcomment" + resName);
    }

    #endregion
}