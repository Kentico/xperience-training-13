using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.MessageBoards.Web.UI;


public partial class CMSModules_MessageBoards_Controls_MessageActions : BoardMessageActions, IPostBackEventHandler
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Initialize control elements
        SetupControl();
    }


    #region "Private methods"

    private void SetupControl()
    {
        // Initialize link button labels
        lnkApprove.Text = GetString("general.approve");
        lnkDelete.Text = GetString("general.delete");
        lnkEdit.Text = GetString("general.edit");
        lnkReject.Text = GetString("general.reject");

        // Set visibility according to the properties
        lnkEdit.Visible = ShowEdit;
        lnkDelete.Visible = ShowDelete;
        lnkApprove.Visible = ShowApprove;
        lnkReject.Visible = ShowReject;

        // Get client script
        ProcessData();

        // Register client script
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteBoardMessageConfirmation", ScriptHelper.GetScript("function ConfirmDelete(){ return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");}"));
    }


    /// <summary>
    /// Generate a client JavaScript for displaying modal window for message editing.
    /// </summary>
    private void ProcessData()
    {
        lnkEdit.OnClientClick = string.Format("EditBoardMessage('{0}?messageid={1}&messageboardid={2}');return false;", ResolveUrl("~/CMSModules/MessageBoards/CMSPages/Message_Edit.aspx"), MessageID, MessageBoardID);
        lnkDelete.OnClientClick = string.Format("if(ConfirmDelete()) {{ {0}; }} return false;", GetPostBackEventReference("delete"));
        lnkApprove.OnClientClick = string.Format("{0}; return false;", GetPostBackEventReference("approve"));
        lnkReject.OnClientClick = string.Format("{0}; return false;", GetPostBackEventReference("reject"));
    }


    private string GetPostBackEventReference(string actionName)
    {
        return ControlsHelper.GetPostBackEventReference(this, string.Format("{0};{1}", actionName, MessageID));
    }

    #endregion


    #region "Public methods"

    public void RaisePostBackEvent(string eventArgument)
    {
        var parts = eventArgument.Split(';');
        FireOnBoardMessageAction(parts[0], parts[1]);
    }

    #endregion
}
