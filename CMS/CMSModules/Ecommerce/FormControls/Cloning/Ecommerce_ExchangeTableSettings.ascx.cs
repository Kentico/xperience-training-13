using System;
using System.Collections;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_Cloning_Ecommerce_ExchangeTableSettings : CloneSettingsControl
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ExchangeTableInfo eti = (ExchangeTableInfo)InfoToClone;
            if (eti != null)
            {
                dtValidFrom.SelectedDateTime = eti.ExchangeTableValidFrom;
                dtValidTo.SelectedDateTime = eti.ExchangeTableValidTo;
            }
        }
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (dtValidFrom.SelectedDateTime > dtValidTo.SelectedDateTime)
        {
            ShowError(GetString("general.dateoverlaps"));
            return false;
        }
        if (!dtValidFrom.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
            return false;
        }
        if (!dtValidTo.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
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
        result[ExchangeTableInfo.OBJECT_TYPE + ".validfrom"] = dtValidFrom.SelectedDateTime;
        result[ExchangeTableInfo.OBJECT_TYPE + ".validto"] = dtValidTo.SelectedDateTime;
        return result;
    }

    #endregion
}