using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

using Action = CMS.UIControls.UniGridConfig.Action;


public partial class CMSModules_AdminControls_Controls_UIControls_Listing : CMSAbstractUIWebpart
{
    #region "Variables"

    private bool? mEditAllowed;
    private BaseInfo mEmptyCurrentInfo;
    private UIElementInfo mUIEdit;
    private bool mEditInDialog;

    #endregion


    #region "Properties"

    /// <summary>
    /// Checks if user is authorized to modify objects.
    /// </summary>
    private bool EditAllowed
    {
        get
        {
            if (mEditAllowed == null)
            {
                mEditAllowed = mEmptyCurrentInfo != null && mEmptyCurrentInfo.CheckPermissions(PermissionsEnum.Delete, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser);
            }

            return mEditAllowed.Value;
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Path for XML grid's definition
    /// </summary>
    public String GridName
    {
        get
        {
            return GetStringContextValue("GridName");
        }
        set
        {
            SetValue("GridName", value);
        }
    }


    /// <summary>
    /// Where condition
    /// </summary>
    public String WhereCondition
    {
        get
        {
            return GetStringContextValue("WhereCondition", String.Empty, true, true);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Zero rows text
    /// </summary>
    public String ZeroRowsText
    {
        get
        {
            return GetStringContextValue("ZeroRowsText");
        }
        set
        {
            SetValue("ZeroRowsText", value);
        }
    }


    /// <summary>
    /// Script to execute after record is deleted.
    /// </summary>
    public String AfterDeleteScript
    {
        get
        {
            return GetStringContextValue("AfterDeleteScript");
        }
        set
        {
            SetValue("AfterDeleteScript", value);
        }
    }


    /// <summary>
    /// Order by statement.
    /// </summary>
    public String OrderBy
    {
        get
        {
            return GetStringContextValue("OrderBy", String.Empty, true, true);
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Javascript after edit is clicked.
    /// </summary>
    public String EditActionUrl
    {
        get
        {
            return GetStringContextValue("EditActionUrl");
        }
        set
        {
            SetValue("EditActionUrl", value);
        }
    }


    /// <summary>
    /// Name of edit element object. If empty first item with 'edit' prefix is used.
    /// </summary>
    public String EditElement
    {
        get
        {
            return GetStringContextValue("EditElement");
        }
        set
        {
            SetValue("EditElement", value);
        }
    }


    /// <summary>
    /// Indicates whether this control should check if user has View permissions to the ObjectType which is displayed. If true, BaseInfo's method CheckPermission is
    /// called to determine whether control can display data. If false, permissions aren't checked and data are always displayed.
    /// </summary>
    public bool CheckInfoViewPermissions
    {
        get
        {
            return GetBoolContextValue("CheckInfoViewPermissions", true);
        }
        set
        {
            SetValue("CheckInfoViewPermissions", value);
        }
    }


    /// <summary>
    /// Indicates whether this control should check if user has Delete permissions to the ObjectType which is displayed. If true, BaseInfo's method CheckPermission is
    /// called to determine whether control can display data. If false, permissions aren't checked and delete button is always displayed.
    /// </summary>
    public bool CheckInfoDeletePermissions
    {
        get
        {
            return GetBoolContextValue("CheckInfoDeletePermissions", true);
        }
        set
        {
            SetValue("CheckInfoDeletePermissions", value);
        }
    }


    /// <summary>
    /// Indicates if grid should offer data export, if not set then value from TypeInfo is used.
    /// </summary>
    private bool AllowDataExport
    {
        get
        {
            return GetBoolContextValue("AllowDataExport", false);
        }
    }

    #endregion


    #region "Life cycle"

    protected override void OnInit(EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            if (!String.IsNullOrEmpty(AfterDeleteScript))
            {
                gridElem.OnAction += gridElem_OnAction;
            }

            // If edit element name is set, load it directly. Otherwise load first element with prefix 'edit' in it's name.
            mUIEdit = String.IsNullOrEmpty(EditElement) ? UIContext.UIElement.GetEditElement() :
                UIElementInfoProvider.GetUIElementInfo(UIContext.UIElement.ElementResourceID, EditElement);

            gridElem.CurrentResolver.SetNamedSourceData("UIContext", UIContext);

            if (mUIEdit != null)
            {
                UIContextData data = new UIContextData();
                data.LoadData(mUIEdit.ElementProperties);
                mEditInDialog = data["OpenInDialog"].ToBoolean(false);

                gridElem.EditInDialog = mEditInDialog;
                gridElem.DialogWidth = data["DialogWidth"].ToString(null);
                gridElem.DialogHeight = data["DialogHeight"].ToString(null);
            }

            // Check modify permission on object type, to disable delete button
            var objectType = ObjectType;

            // Pass element's object type to unigrid. It may be overridden with unigrid definition
            if (!String.IsNullOrEmpty(objectType))
            {
                gridElem.ObjectType = objectType;
            }

            mEmptyCurrentInfo = gridElem.InfoObject;

            // Try to fake siteID (if object has siteID column)
            if ((mEmptyCurrentInfo != null) && (mEmptyCurrentInfo.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
            {
                mEmptyCurrentInfo.Generalized.ObjectSiteID = GetSiteID(mEmptyCurrentInfo);
            }

            // Apply delete check only for non global admins
            if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
                gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            }

            if (AllowDataExport)
            {
                gridElem.ShowExportMenu = AllowDataExport;
            }

            // Must be called before permission check to initialize grid extenders properly
            base.OnInit(e);

            // Check view permission for object type
            if (CheckInfoViewPermissions && !CheckViewPermissions(mEmptyCurrentInfo))
            {
                gridElem.StopProcessing = true;
                gridElem.Visible = false;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            String defDir = GetDefaultGridDirectory();

            string gridName = GridName;
            // If grid name is not set, find in default directory
            if (String.IsNullOrEmpty(gridName))
            {
                gridName = String.Format("{0}/{1}", defDir, "default.xml");
            }
            else if (!gridName.StartsWith("~", StringComparison.Ordinal))
            {
                // If only grid name is specified (not path), append name to default directory
                gridName = String.Format("{0}/{1}", defDir, gridName);
            }

            // Check the configuration file
            String path = Server.MapPath(gridName);
            if (!File.Exists(path))
            {
                ShowError(String.Format(GetString("unigrid.noxmluifile"), path));
                return;
            }

            gridElem.GridName = gridName;

            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                gridElem.ZeroRowsText = ResHelper.LocalizeString(ZeroRowsText);
            }

            if (!String.IsNullOrEmpty(WhereCondition))
            {
                gridElem.WhereCondition = WhereCondition;
            }

            if (!String.IsNullOrEmpty(OrderBy))
            {
                gridElem.OrderBy = OrderBy;
            }
            // Default order by - use object's display name
            else if (mEmptyCurrentInfo != null)
            {
                var obj = EditedObject as BaseInfo;

                if ((obj == null) || (obj.TypeInfo.ObjectType != ObjectType))
                {
                    // Sort by order column, if not defined, use display name
                    gridElem.OrderBy = mEmptyCurrentInfo.TypeInfo.DefaultOrderBy;
                }
            }

            // Set edit script or redirect
            gridElem.EditActionUrl = !String.IsNullOrEmpty(EditActionUrl) ? ResolveUrl(EditActionUrl) : GetEditUrl();
        }
    }

    #endregion


    #region "Page events"

    private void gridElem_OnBeforeDataReload()
    {
        var ug = gridElem.GridActions;
        if (ug != null)
        {
            // Find delete
            foreach (var abstractAction in ug.Actions)
            {
                var action = abstractAction as Action;
                if ((action != null) && String.IsNullOrEmpty(action.ExternalSourceName))
                {
                    switch (action.Name.ToLowerInvariant())
                    {
                        case "delete":
                        case "#delete":
                            action.ExternalSourceName = "delete";
                            return;
                    }
                }
            }
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "delete":
            case "#delete":
                if (CheckInfoDeletePermissions && !EditAllowed)
                {
                    var button = sender as CMSGridActionButton;
                    if (button != null)
                    {
                        button.Enabled = false;
                        break;
                    }

                    // Web part supports custom png icons
                    var imgDel = sender as ImageButton;
                    if (imgDel != null)
                    {
                        imgDel.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/deletedisabled.png");
                        imgDel.Enabled = false;
                        imgDel.Style.Add("cursor", "default");
                    }
                }
                break;
        }

        return parameter;
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
            case "#delete":
                if (!String.IsNullOrEmpty(AfterDeleteScript))
                {
                    ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "UniGridAfterScript", ScriptHelper.GetScript(AfterDeleteScript));
                }
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns default grid directory based on object type and module name.
    /// </summary>    
    private String GetDefaultGridDirectory()
    {
        String resourceName = ResourceName;
        if (resourceName.StartsWith("cms.", StringComparison.OrdinalIgnoreCase))
        {
            resourceName = resourceName.Substring(4);
        }

        return String.Format("{0}{1}/UI/Grids/{2}", UIContextHelper.GRIDDIRECTORY, resourceName, ObjectType.Replace(".", "_"));
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        String url = String.Empty;

        if (mUIEdit != null)
        {
            url = UIContextHelper.GetElementUrl(mUIEdit, UIContext);
            url = URLHelper.AppendQuery(url, "objectid={0}");

            // Ensures dialog parameters
            if (mEditInDialog)
            {
                url = URLHelper.UpdateParameterInUrl(url, "dialog", "true");
                url = URLHelper.UpdateParameterInUrl(url, "displaytitle", "true");
            }

            // Add specific ID for object. F.e. add ReportID={ID} to query
            String objectParameterID = ValidationHelper.GetString(UIContext["objectParameterID"], String.Empty);
            if (objectParameterID != String.Empty)
            {
                url += String.Format("&{0}={{0}}", objectParameterID);
            }

            url = UIContextHelper.AppendDialogHash(UIContext, url);
        }

        return url;
    }

    #endregion
}