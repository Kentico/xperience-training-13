using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Automation_Pages_Comment : CMSModalPage
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


    /// <summary>
    /// Menu ID
    /// </summary>
    protected string MenuID
    {
        get
        {
            return QueryHelper.GetString("menuid", null);
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        autoMan.StateObjectID = QueryHelper.GetInteger("stateId", 0);
        ucComment.ActionName = ActionName;
        ucComment.MenuID = MenuID;
    }


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
            ShowError(GetString("ma.notauthorizedaction"));
        }
    }


    private void InitHeader()
    {
        // Set title
        string resName = HTMLHelper.HTMLEncode(ActionName);
        PageTitle.TitleText = GetString("editmenu.iconcomment" + resName);
    }

    #endregion
}