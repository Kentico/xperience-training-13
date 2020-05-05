using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Filters_DocTypeFilter : CMSAbstractBaseFilterControl
{
    private const string DOCUMENT_TYPE = "doctype";
    private const string CUSTOM_TABLE = "customtables";

    private bool? mIsSiteManager;
    
    private CMSUserControl mFilteredUserControl;


    /// <summary>
    /// If false, custom table in drop down is never shown.
    /// </summary>
    public bool ShowCustomTableClasses
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Gets selected class id.
    /// </summary>
    public int ClassId => ValidationHelper.GetInteger(uniSelector.Value, 0);


    /// <summary>
    /// If true, filter is in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            if (mIsSiteManager == null)
            {
                var parent = ControlsHelper.GetParentControl(this, typeof(CMSUserControl)) as CMSUserControl;
                if (parent != null)
                {
                    mIsSiteManager = ValidationHelper.GetBoolean(parent.GetValue("IsSiteManager"), false)
                                     && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
                }
                else
                {
                    mIsSiteManager = false;
                }
            }

            return mIsSiteManager.Value;
        }
        set
        {
            mIsSiteManager = value;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                drpClassType.Items.Clear();
                drpClassType.Items.Add(new ListItem(ResHelper.GetString("general.documenttype"), DOCUMENT_TYPE));

                if (ShowCustomTableClasses && CustomTableHelper.GetCustomTableClasses(SiteContext.CurrentSiteID).HasResults())
                {
                    drpClassType.Items.Add(new ListItem(ResHelper.GetString("queryselection.classtype.customtables"), CUSTOM_TABLE));
                }

                Initialize();
            }

            SetupSelector();

            WhereCondition = GenerateWhereCondition();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblDocType.AssociatedControlClientID = uniSelector.InputClientID;
        lblDocType.ResourceString = drpClassType.SelectedValue == DOCUMENT_TYPE ? "queryselection.lbldoctypes" : "queryselection.customtable";
    }


    /// <summary>
    /// Setups the selector for further usage.
    /// </summary>
    private void SetupSelector()
    {
        mFilteredUserControl = FilteredControl as CMSUserControl;

        SetSelectorWhereCondition();

        uniSelector.ObjectType = DataClassInfo.OBJECT_TYPE;
        uniSelector.DropDownSingleSelect.AutoPostBack = true;
        uniSelector.IsLiveSite = IsLiveSite && ((mFilteredUserControl == null) || mFilteredUserControl.IsLiveSite);
    }


    /// <summary>
    /// Initialize filter.
    /// </summary>
    private void Initialize()
    {
        if (!String.IsNullOrEmpty(SelectedValue))
        {
            DataClassInfo selectedClass = null;

            switch (FilterMode)
            {
                case TransformationInfo.OBJECT_TYPE:
                    var transformation = TransformationInfoProvider.GetTransformation(SelectedValue);
                    if (transformation != null)
                    {
                        selectedClass = DataClassInfoProvider.GetDataClassInfo(transformation.TransformationClassID);
                    }
                    break;

                case QueryInfo.OBJECT_TYPE:
                    var query = QueryInfoProvider.GetQueryInfo(SelectedValue, false);
                    if (query != null)
                    {
                        selectedClass = DataClassInfoProvider.GetDataClassInfo(query.ClassID);
                    }
                    break;
            }

            // If selected object is under custom class, change selected class type
            if (selectedClass != null)
            {
                if (selectedClass.ClassIsCustomTable)
                {
                    var selectedItem = ControlsHelper.FindItemByValue(drpClassType, CUSTOM_TABLE, false);

                    // Select item which is already loaded in drop-down list
                    if (selectedItem != null)
                    {
                        drpClassType.SelectedValue = selectedItem.Value;
                    }
                }
                else if (!selectedClass.ClassIsDocumentType)
                {
                    return;
                }

                uniSelector.Value = selectedClass.ClassID;
            }
        }
    }


    /// <summary>
    /// Main filter action (document name changed). Raises the filter event.
    /// </summary>
    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        WhereCondition = GenerateWhereCondition();
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Class type changed event.
    /// </summary>
    protected void drpClassType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetSelectorWhereCondition();

        uniSelector.Reload(true);

        WhereCondition = GenerateWhereCondition();
        RaiseOnFilterChanged();
    }


    private void SetSelectorWhereCondition()
    {
        uniSelector.WhereCondition = drpClassType.SelectedValue == DOCUMENT_TYPE ? "ClassIsDocumentType = 1" : "ClassIsCustomTable = 1";

        if (!IsSiteManager)
        {
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, $"ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = {SiteContext.CurrentSiteID})");
        }
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        if (!uniSelector.HasData)
        {
            return SqlHelper.NO_DATA_WHERE;
        }

        var classId = ValidationHelper.GetInteger(uniSelector.Value, 0);
        if (classId <= 0)
        {
            return SqlHelper.NO_DATA_WHERE;
        }

        // Only results for specific class
        var mode = string.Empty;
        if (mFilteredUserControl != null)
        {
            mode = ValidationHelper.GetString(mFilteredUserControl.GetValue("FilterMode"), "");
            // Set the prefix for the item
            var className = DataClassInfoProvider.GetClassName(classId);
            mFilteredUserControl.SetValue("ItemPrefix", className + ".");
        }

        var id = ValidationHelper.GetInteger(uniSelector.Value, 0);
        switch (mode.ToLowerInvariant())
        {
            case TransformationInfo.OBJECT_TYPE:
                return $"(TransformationClassID = {id})";

            default:
                return $"(ClassID = {id})";
        }
    }
}