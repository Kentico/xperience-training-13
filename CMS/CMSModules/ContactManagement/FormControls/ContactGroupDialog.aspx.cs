using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_ContactGroupDialog : CMSModalPage
{
    #region "Variables"

    protected Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Stop processing flag.
    /// </summary>
    public bool StopProcessing
    {
        get
        {
            return gridElem.StopProcessing;
        }
        set
        {
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
        PageTitle.TitleText = GetString("om.contactgroup.selecttitle");
        Page.Title = PageTitle.TitleText;

        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            StopProcessing = true;
            return;
        }

        // Check permission
        if (AuthorizationHelper.AuthorizedReadContact(true))
        {
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        }
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if(sourceName.Equals("contactgroupdisplayname", StringComparison.InvariantCultureIgnoreCase))
        {        
            LinkButton btn = new LinkButton();
            DataRowView drv = parameter as DataRowView;
            btn.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(drv["ContactGroupDisplayName"], null)));
            btn.Click += btn_Click;
            btn.CommandArgument = ValidationHelper.GetString(drv["ContactGroupID"], null);
            btn.ToolTip = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["ContactGroupDescription"], null));
            return btn;
        }
        return parameter;
    }


    /// <summary>
    /// Contact group selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int groupId = ValidationHelper.GetInteger(((LinkButton)sender).CommandArgument, 0);
        string script = ScriptHelper.GetScript(@"
wopener.SelectValue_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + groupId + @");
CloseDialog();
");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseWindow", script);
    }

    #endregion
}
