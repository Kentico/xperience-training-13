using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Membership_FormControls_Membership_SelectMembership : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = -1;
    private bool mUseCodeNameForSelection = true;
    private bool mUseGUIDForSelection = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets membership ID.
    /// </summary>
    public int MembershipID
    {
        get
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                Guid guid = ValidationHelper.GetGuid(uniSelectorElem.Value, Guid.Empty);

                MembershipInfo mi = MembershipInfo.Provider.Get(guid, SiteID);

                return (mi != null) ? mi.MembershipID : 0;
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                string name = ValidationHelper.GetString(uniSelectorElem.Value, String.Empty);

                MembershipInfo mi = MembershipInfo.Provider.Get(name, SiteID);

                return (mi != null) ? mi.MembershipID : 0;
            }
            // Use ID
            else
            {
                return ValidationHelper.GetInteger(uniSelectorElem.Value, 0);
            }
        }
        set
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipGUID;
                }
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipName;
                }
            }
            // Use ID
            else
            {
                uniSelectorElem.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets membership GUID.
    /// </summary>
    public Guid MembershipGUID
    {
        get
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                return ValidationHelper.GetGuid(uniSelectorElem.Value, Guid.Empty);
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                string name = ValidationHelper.GetString(uniSelectorElem.Value, String.Empty);

                MembershipInfo mi = MembershipInfo.Provider.Get(name, SiteID);

                return (mi != null) ? mi.MembershipGUID : Guid.Empty;
            }
            // Use ID
            else
            {
                int id = ValidationHelper.GetInteger(uniSelectorElem.Value, 0);

                MembershipInfo mi = MembershipInfo.Provider.Get(id);

                return (mi != null) ? mi.MembershipGUID : Guid.Empty;
            }
        }
        set
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                uniSelectorElem.Value = value;
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value, SiteID);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipName;
                }
            }
            // Use ID
            else
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value, SiteID);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipID;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets membership code name.
    /// </summary>
    public string MembershipName
    {
        get
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                Guid guid = ValidationHelper.GetGuid(uniSelectorElem.Value, Guid.Empty);

                MembershipInfo mi = MembershipInfo.Provider.Get(guid, SiteID);

                return (mi != null) ? mi.MembershipName : String.Empty;
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                return ValidationHelper.GetString(uniSelectorElem.Value, String.Empty);
            }
            // Use ID
            else
            {
                int id = ValidationHelper.GetInteger(uniSelectorElem.Value, 0);

                MembershipInfo mi = MembershipInfo.Provider.Get(id);

                return (mi != null) ? mi.MembershipName : String.Empty;
            }
        }
        set
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value, SiteID);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipGUID;
                }
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                uniSelectorElem.Value = value;
            }
            // Use ID
            else
            {
                MembershipInfo mi = MembershipInfo.Provider.Get(value, SiteID);

                if (mi != null)
                {
                    uniSelectorElem.Value = mi.MembershipID;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                return MembershipGUID;
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                return MembershipName;
            }
            // Use ID
            else
            {
                return MembershipID;
            }
        }
        set
        {
            EnsureChildControls();

            // Use GUID
            if (UseGUIDForSelection)
            {
                MembershipGUID = ValidationHelper.GetGuid(value, Guid.Empty);
            }
            // Use code name
            else if (UseCodeNameForSelection)
            {
                MembershipName = ValidationHelper.GetString(value, String.Empty);
            }
            // Use ID
            else
            {
                MembershipID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets client ID of the dropdown list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();

            return uniSelectorElem.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add "none" option to dropdown list.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddNoneRecord"), mAddNoneRecord);
        }
        set
        {
            SetValue("AddNoneRecord", value);
            mAddNoneRecord = value;
        }
    }
    private bool mAddNoneRecord = false;


    /// <summary>
    /// Gets or sets enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();

            base.Enabled = value;

            if (uniSelectorElem != null)
            {
                uniSelectorElem.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Indicates if membership GUID is used for selection.
    /// </summary>
    public bool UseGUIDForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseGUIDForSelection"), mUseGUIDForSelection);
        }
        set
        {
            SetValue("UseGUIDForSelection", value);
            mUseGUIDForSelection = value;

            // If setting to true
            if (value)
            {
                // Set other options to false
                mUseCodeNameForSelection = false;
            }
        }
    }


    /// <summary>
    /// Indicates if membership code name is used for selection.
    /// </summary>
    public bool UseCodeNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCodeNameForSelection"), mUseCodeNameForSelection);
        }
        set
        {
            SetValue("UseCodeNameForSelection", value);
            mUseCodeNameForSelection = value;

            // If setting to true
            if (value)
            {
                // Set other options to false
                mUseGUIDForSelection = !value;
            }
        }
    }


    /// <summary>
    /// Allows to display memberships only for specified site ID. Use 0 for global memberships. Default value is current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId == -1)
            {
                return SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }

    #endregion


    #region "Lifecycle methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Membership))
        {
            StopProcessing = true;
            return;
        }

        TryInitByForm();
        InitSelector();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        uniSelectorElem.Reload(true);
    }

    #endregion


    #region "Initialization methods"

    private void TryInitByForm()
    {
        if (Form == null)
        {
            return;
        }

        if (Form.Data.ContainsColumn("SKUSiteID"))
        {
            SiteID = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
        }
    }


    /// <summary>
    /// Initliazes the selector control.
    /// </summary>
    protected void InitSelector()
    {
        uniSelectorElem.IsLiveSite = IsLiveSite;
        uniSelectorElem.AllowEmpty = AddNoneRecord;

        // Use GUID
        if (UseGUIDForSelection)
        {
            uniSelectorElem.ReturnColumnName = "MembershipGUID";
        }
        // Use code name
        else if (UseCodeNameForSelection)
        {
            uniSelectorElem.ReturnColumnName = "MembershipName";
        }
        // Use ID
        else
        {
            uniSelectorElem.ReturnColumnName = "MembershipID";
        }

        // Set up where condition
        string where = "";

        if (SiteID > 0)
        {
            // Add site items
            where = SqlHelper.AddWhereCondition(where, String.Format("(MembershipSiteID = {0})", SiteID));
        }
        else
        {
            // Add global items
            where = SqlHelper.AddWhereCondition(where, "(MembershipSiteID IS NULL)");
        }

        // Ensure selected item
        if (MembershipID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, String.Format("(MembershipID = {0})", MembershipID), "OR");
        }

        // Set where condition
        uniSelectorElem.WhereCondition = where;
    }

    #endregion
}