using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_AsyncLogDialog : AsyncControl
{
    #region "Properties"

    /// <summary>
    /// Title of the modal dialog.
    /// </summary>
    public string TitleText
    {
        get
        {
            return ctlTitle.TitleText;
        }
        set
        {
            ctlTitle.TitleText = value;
        }
    }


    /// <summary>
    /// Title element of the modal dialog.
    /// </summary>
    public PageTitle Title
    {
        get
        {
            return ctlTitle;
        }
    }


    /// <summary>
    /// Body of the modal dialog.
    /// </summary>
    public Panel Body
    {
        get
        {
            return pnlBody;
        }
    }


    /// <summary>
    /// Header of the modal dialog.
    /// </summary>
    public Panel Header
    {
        get
        {
            return pnlHeader;
        }
    }


    /// <summary>
    /// Button panel of the modal dialog.
    /// </summary>
    public Panel Buttons
    {
        get
        {
            return pnlFooter;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        LogPanel = pnlLog;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register dialog script
        btnCancelAction.OnClientClick = GetCancelScript(true) + "return false;";
        ScriptHelper.RegisterJQueryDialog(Page);
        string resizeScript = "showModalBackground('" + pnlAsyncBackground.ClientID + "');";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "asyncBackground" + ClientID, ScriptHelper.GetScript(resizeScript));
    }

    #endregion
}
