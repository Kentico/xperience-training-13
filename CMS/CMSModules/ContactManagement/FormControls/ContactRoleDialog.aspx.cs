using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_ContactRoleDialog : CMSModalPage
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
        if (!QueryHelper.ValidateHash("hash") || Parameters == null)
        {
            StopProcessing = true;
            return;
        }

        PageTitle.TitleText = GetString("om.contactrole.selectitem");
        Page.Title = PageTitle.TitleText;

        // Check read permission
        if (!AuthorizationHelper.AuthorizedReadConfiguration(false))
        {
            RedirectToAccessDenied("cms.contactmanagement", "ReadConfiguration");
            return;
        }

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.Pager.DefaultPageSize = 10;

        // Display 'Reset' button when 'none' role is allowed
        if (ValidationHelper.GetBoolean(Parameters["allownone"], false))
        {
            btnReset.Visible = true;
            btnReset.Click += btn_Click;
            btnReset.CommandArgument = "0";
        }
    }


    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "contactroledisplayname":
                LinkButton btn = new LinkButton();
                DataRowView drv = parameter as DataRowView;
                btn.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(drv["ContactRoleDisplayName"], null)));
                btn.Click += btn_Click;
                btn.CommandArgument = ValidationHelper.GetString(drv["ContactRoleID"], null);
                btn.ToolTip = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["ContactRoleDescription"], null));
                return btn;
        }
        return parameter;
    }


    /// <summary>
    /// Contact role selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int roleId = ValidationHelper.GetInteger(((IButtonControl)sender).CommandArgument, 0);
        string script = null;
        if (!Parameters.ContainsKey("ismassaction"))
        {
            SetContactRoleToAccount(roleId, ValidationHelper.GetInteger(Parameters["accountcontactid"], 0));

            script = ScriptHelper.GetScript(@"
if (wopener.Refresh) {wopener.Refresh();}
setTimeout('CloseDialog()',200);
");
        }
        else
        {
            script = ScriptHelper.GetScript(@"
wopener.AssignContactRole_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + roleId + @");
setTimeout('CloseDialog()',200);
");
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseWindow", script);
    }


    private void SetContactRoleToAccount(int roleId, int accountContactId)
    {
        var aci = AccountContactInfo.Provider.Get(accountContactId);
        if (aci != null)
        {
            aci.ContactRoleID = roleId;
            AccountContactInfo.Provider.Set(aci);
        }
    }

    #endregion
}
