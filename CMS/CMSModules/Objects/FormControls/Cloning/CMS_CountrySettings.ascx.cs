using System;
using System.Collections;
using System.Data;

using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_CountrySettings : CloneSettingsControl
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
            CountryInfo country = (CountryInfo) InfoToClone;
            txtTwoLetterCode.Text = country.CountryTwoLetterCode;
            txtThreeLetterCode.Text = country.CountryThreeLetterCode;
        }
    }


    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        DataSet ds = CountryInfoProvider.GetCountries().WhereEquals("CountryTwoLetterCode", txtTwoLetterCode.Text).Or().WhereEquals("CountryThreeLetterCode", txtThreeLetterCode.Text);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ShowError(GetString("clonning.settings.country.uniquecodes"));
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
        result[CountryInfo.OBJECT_TYPE + ".twolettercode"] = txtTwoLetterCode.Text;
        result[CountryInfo.OBJECT_TYPE + ".threelettercode"] = txtThreeLetterCode.Text;
        return result;
    }

    #endregion
}