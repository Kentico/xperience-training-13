using System;
using System.Collections;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;


public partial class CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        rfvEmail.Text = GetString("om.contact.enteremail");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        txtEmail.RegisterCustomValidator(rfvEmail);

        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = InfoToClone.GetStringValue("ReportSubscriptionEmail", "");
        }
    }


    /// <summary>
    /// Returns if input value is valid email address.
    /// </summary>
    public override bool IsValid(CloneSettings settings)
    {
        if (String.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.IsValid())
        {
            ShowError(GetString("om.contact.enteremail"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ReportSubscriptionInfo.OBJECT_TYPE + ".email"] = txtEmail.Text;
        return result;
    }

    #endregion
}