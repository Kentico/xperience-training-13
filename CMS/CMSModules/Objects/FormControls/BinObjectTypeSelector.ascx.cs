using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Synchronization;


public partial class CMSModules_Objects_FormControls_BinObjectTypeSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// User ID to restrict object types to user recycle bin only.
    /// </summary>
    public int UserID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if all item should be displayed.
    /// </summary>
    public bool ShowAll
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Returns ClientID of the CMSDropDownList with order.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return drpObjectTypes.ClientID;
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
            drpObjectTypes.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpObjectTypes.SelectedValue, "");
        }
        set
        {
            var objType = ValidationHelper.GetString(value, "");
            
            Reload(false);

            drpObjectTypes.ClearSelection();
            var selectedItem = drpObjectTypes.Items.FindByValue(objType);
            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }
        }
    }


    /// <summary>
    /// Where condition to filter values.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Drop-down list control.
    /// </summary>
    public CMSDropDownList DropDownListControl
    {
        get
        {
            return drpObjectTypes;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Ensure creating child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        Reload(false);
    }


    /// <summary>
    /// Reload DDL content.
    /// </summary>
    /// <param name="force">Indicates if DDL reload should be forced</param>
    public void Reload(bool force)
    {
        if ((drpObjectTypes.Items.Count == 0) || force)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Loads drop down list with data.
    /// </summary>
    private void ReloadData()
    {
        drpObjectTypes.Items.Clear();

        // Check if show all item should be displayed
        if (ShowAll)
        {
            drpObjectTypes.Items.Add(new ListItem(GetString("general.selectall"), ""));
        }

        // Get recycle bin object types
        DataSet dsObjectTypes = ObjectVersionHistoryInfoProvider.GetRecycleBin(UserID, WhereCondition, "VersionObjectType", -1, "DISTINCT VersionObjectType");
        if (!DataHelper.DataSourceIsEmpty(dsObjectTypes))
        {
            SortedDictionary<string, string> sdObjectTypes = new SortedDictionary<string, string>();
            foreach (DataRow dr in dsObjectTypes.Tables[0].Rows)
            {
                string objType = ValidationHelper.GetString(dr["VersionObjectType"], null);
                if (!String.IsNullOrEmpty(objType))
                {
                    // Sort object types by translated display names
                    sdObjectTypes.Add(objType, GetString("ObjectType." + objType.Replace(".", "_")));
                }
            }

            foreach (string key in sdObjectTypes.Keys)
            {
                drpObjectTypes.Items.Add(new ListItem(sdObjectTypes[key], key));
            }
        }
    }

    #endregion
}