using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Classes_CustomTableSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or stes the value that indicates whether selector should display 
    /// all custom tables without restriction to the current site.
    /// </summary>
    public bool AllSites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllSites"), false);
        }
        set
        {
            SetValue("AllSites", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with classnames.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets inner uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets dropdown list.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            EnsureChildControls();
            return uniSelector.DropDownSingleSelect;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }

    #endregion


    protected override void EnsureChildControls()
    {
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        base.EnsureChildControls();
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        if (!AllSites)
        {
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, "ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")");
        }
        uniSelector.Reload(false);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        string[] values = ValidationHelper.GetString(uniSelector.Value, string.Empty).Split(new char[] { ';' });
        foreach (string className in values)
        {
            if ((className != string.Empty) && !MacroProcessor.ContainsMacro(className))
            {
                DataClassInfo di = DataClassInfoProvider.GetDataClassInfo(className);
                if (di == null)
                {
                    ValidationError = GetString("formcontrols_selectcustomtable.notexist").Replace("%%code%%", className);
                    return false;
                }
            }
        }
        return true;
    }
}