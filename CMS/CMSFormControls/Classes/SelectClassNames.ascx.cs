using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Classes_SelectClassNames : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Underlying control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return uniSelector;
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
            return uniSelector.TextBoxSelect.ClientID;
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


    /// <summary>
    /// Specifies, whether the selector allows empty selection.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return GetValue("AllowEmpty", true);
        }
        set
        {
            SetValue("AllowEmpty", value);
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows selection of all items.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return GetValue("AllowAll", false);
        }
        set
        {
            SetValue("AllowAll", value);
        }
    }


    /// <summary>
    /// Specifies aditional where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return GetValue("WhereCondition", string.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Gets or sets the SiteID value to filter classnames. Zero value means current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return GetValue("SiteID", 0);
        }
        set
        {
            SetValue("SiteID", value);
        }
    }


    /// <summary>
    /// Indicates if should be shown only document types.
    /// </summary>
    public bool ShowOnlyCoupled
    {
        get
        {
            return GetValue("ShowOnlyCoupled", false);
        }
        set
        {
            SetValue("ShowOnlyCoupled", value);
        }
    }


    /// <summary>
    /// Indicates if should be shown only system tables.
    /// </summary>
    public bool ShowOnlySystemTables
    {
        get
        {
            return GetValue("ShowOnlySystemTables", false);
        }
        set
        {
            SetValue("ShowOnlySystemTables", value);
        }
    }


    /// <summary>
    /// Enables / disables the multiple selection mode.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return (SelectionModeEnum)ValidationHelper.GetInteger(GetValue("SelectionMode"), (int)SelectionModeEnum.MultipleTextBox);
        }
        set
        {
            SetValue("SelectionMode", (int)value);
        }
    }


    /// <summary>
    /// Column name of the object which value should be returned by the selector. 
    /// If NULL, ID column is used.
    /// </summary>
    public virtual string ReturnColumnName
    {
        get
        {
            return GetValue("ReturnColumnName", "ClassName");
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Indicates if control should hide inherited classes. Applies only in UIForm.
    /// </summary>
    private bool HideInheritedClasses
    {
        get
        {
            return GetValue("HideInheritedClasses", false);
        }
        set
        {
            SetValue("HideInheritedClasses", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData(false);
        }
    }


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
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public void ReloadData(bool forceReload)
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.SelectionMode = SelectionMode;
        uniSelector.ReturnColumnName = ReturnColumnName;

        // Where condition
        string where = null;

        // Show only document types
        if (ShowOnlyCoupled)
        {
            where = "ClassIsCoupledClass = 1";
        }

        if ((Form != null) && HideInheritedClasses)
        {
            DataClassInfo currentClass = (DataClassInfo)Form.EditedObject;
            if (currentClass != null)
            {
                where = SqlHelper.AddWhereCondition(where, "ClassID <> " + currentClass.ClassID + " AND (ClassInheritsFromClassID IS NULL OR ClassInheritsFromClassID <> " + currentClass.ClassID + ")");
            }
        }

        // Show only system tables
        if (ShowOnlySystemTables)
        {
            where = SqlHelper.AddWhereCondition(where, "ClassShowAsSystemTable = 1");
        }

        // Filter using Site ID
        int siteId = (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID;
        if ((siteId > 0) && !ShowOnlySystemTables)
        {
            where = SqlHelper.AddWhereCondition(where, string.Format("ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = {0})", siteId));
        }

        if (!string.IsNullOrEmpty(WhereCondition))
        {
            where = SqlHelper.AddWhereCondition(where, WhereCondition);
        }

        uniSelector.WhereCondition = where;
        uniSelector.Reload(forceReload);
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (ReturnColumnName.EqualsCSafe("ClassName", true))
        {
            string[] values = ValidationHelper.GetString(uniSelector.Value, "").Split(new[] { ';' });
            foreach (string className in values)
            {
                if ((className != "") && !MacroProcessor.ContainsMacro(className) && !className.Contains("*"))
                {
                    DataClassInfo di = DataClassInfoProvider.GetDataClassInfo(className);
                    if (di == null)
                    {
                        ValidationError = GetString("formcontrols_selectclassnames.notexist").Replace("%%code%%", className);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    #endregion
}