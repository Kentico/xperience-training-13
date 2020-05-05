using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_AccountStatusDialog : CMSModalPage
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

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
        PageTitle.TitleText = GetString("om.accountstatus.select");
        Page.Title = PageTitle.TitleText;

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.Pager.DefaultPageSize = 10;

        // Display 'Reset' button when 'none' status is allowed
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
        switch (sourceName)
        {
            case "accountstatusdisplayname":
                LinkButton btn = new LinkButton();
                DataRowView drv = parameter as DataRowView;
                btn.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(drv["AccountStatusDisplayName"], null)));
                btn.Click += btn_Click;
                btn.CommandArgument = ValidationHelper.GetString(drv["AccountStatusID"], null);
                btn.ToolTip = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["AccountStatusDescription"], null));
                return btn;
        }
        return parameter;
    }


    /// <summary>
    /// Account status selected event handler.
    /// </summary>
    protected void btn_Click(object sender, EventArgs e)
    {
        int statusId = ValidationHelper.GetInteger(((IButtonControl)sender).CommandArgument, 0);
        string script = ScriptHelper.GetScript(@"
wopener.SelectValue_" + ValidationHelper.GetString(Parameters["clientid"], string.Empty) + @"(" + statusId + @");
CloseDialog();
");

        ScriptHelper.RegisterStartupScript(Page, typeof (string), "CloseWindow", script);
    }

    #endregion
}
