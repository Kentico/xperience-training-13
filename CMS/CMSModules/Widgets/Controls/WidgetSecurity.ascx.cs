using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Widgets_Controls_WidgetSecurity : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private bool mNoRolesAvailable = false;

    private WidgetInfo mWidgetInfo = null;
    private ResourceInfo mResWidget = null;

    // HashTable holding information on all permissions that 'OnlyAuthorizedRoles' access is selected for
    private Hashtable onlyAuth = new Hashtable();

    private ArrayList permissionArray = new ArrayList();

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current widget info.
    /// </summary>
    private WidgetInfo WidgetInfo
    {
        get
        {
            if ((mWidgetInfo == null) && (WidgetID > 0))
            {
                mWidgetInfo = WidgetInfoProvider.GetWidgetInfo(WidgetID);
            }
            return mWidgetInfo;
        }
    }


    /// <summary>
    /// Current widget resource info.
    /// </summary>
    private ResourceInfo ResWidget
    {
        get
        {
            if (mResWidget == null)
            {
                mResWidget = ResourceInfo.Provider.Get("CMS.Widgets");
            }
            return mResWidget;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the widget to edit.
    /// </summary>
    public int WidgetID
    {
        get
        {
            return ItemID;
        }
        set
        {
            ItemID = value;
            mWidgetInfo = null;
        }
    }


    /// <summary>
    /// Indicates whether permissions matrix is enabled.
    /// </summary>
    public bool Enable { get; set; } = true;

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        if (WidgetInfo != null)
        {
            // Render permission matrix
            CreateMatrix();
        }

        // Disable control if needed
        if (!Enable)
        {
            ltlScript.Text = "";
            tblMatrix.Enabled = false;
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (!Visible)
        {
            EnableViewState = false;
        }

        if (WidgetInfo != null)
        {
            gridMatrix.NoRecordsMessage = GetString("general.norolesinsite");

            siteSelector.AllowGlobal = true;
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.UniSelector.OnSelectionChanged += new EventHandler(UniSelector_OnSelectionChanged);
            siteSelector.AllowEmpty = false;

            int siteId = 0;
            if (!RequestHelper.IsPostBack())
            {
                siteId = SiteContext.CurrentSiteID;

                // Site may be stopped, get truly selected value
                if (siteId == 0)
                {
                    siteSelector.Reload(false);
                    siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
                }
                else
                {
                    siteSelector.Value = siteId;
                }
            }
            else
            {
                siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
            }

            // If global role is selected - set SiteID to 0
            if (siteId.ToString() == siteSelector.GlobalRecordValue)
            {
                siteId = 0;
            }


            // Set editable permissions
            permissionArray.Add("allowedfor");

            if (ResWidget != null)
            {
                // Retrive permission matrix data
                QueryDataParameters parameters = new QueryDataParameters();
                parameters.Add("@ID", ResWidget.ResourceID);
                parameters.Add("@WidgetID", WidgetID);
                parameters.Add("@SiteID", siteId);

                string where = null;

                if (permissionArray != null)
                {
                    where = "PermissionName IN (";
                    foreach (string permission in permissionArray)
                    {
                        where += "'" + permission + "',";
                    }
                    where = where.TrimEnd(',');
                    where += ") ";
                }

                // Setup matrix control            
                gridMatrix.QueryParameters = parameters;
                gridMatrix.WhereCondition = where;
                gridMatrix.CssClass = "permission-matrix";
                gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
            }
        }
        else
        {
            Visible = false;
            gridMatrix.StopProcessing = true;
        }
    }


    /// <summary>
    /// Site change.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Sets pager to first page
        gridMatrix.ResetPager();

        pnlUpdate.Update();
    }


    /// <summary>
    /// Generates the permission matrix for the cutrrent widget.
    /// </summary>
    private void CreateMatrix()
    {
        // Get widget resource info             
        if ((ResWidget != null) && (WidgetInfo != null))
        {
            // Get permissions for the current widget resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(ResWidget.ResourceID);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                lblInfo.Text = GetString("general.emptymatrix");
            }
            else
            {
                TableRow headerRow = new TableRow();
                headerRow.CssClass = "unigrid-head";
                headerRow.TableSection = TableRowSection.TableHeader;
                headerRow.HorizontalAlign = HorizontalAlign.Left;
                TableHeaderCell newHeaderCell = new TableHeaderCell();
                newHeaderCell.CssClass = "first-column";
                headerRow.Cells.Add(newHeaderCell);

                DataView dv = permissions.Tables[0].DefaultView;
                dv.Sort = "PermissionName ASC";

                // Generate header cells                
                foreach (DataRowView drv in dv)
                {
                    string permissionName = drv.Row["PermissionName"].ToString();
                    if (permissionArray.Contains(permissionName.ToLowerCSafe()))
                    {
                        newHeaderCell = new TableHeaderCell();
                        newHeaderCell.CssClass = "matrix-header";
                        newHeaderCell.Text = HTMLHelper.HTMLEncode(drv.Row["PermissionDisplayName"].ToString());
                        newHeaderCell.ToolTip = Convert.ToString(drv.Row["PermissionDescription"]);

                        headerRow.Cells.Add(newHeaderCell);
                    }
                }

                tblMatrix.Rows.AddAt(0, headerRow);

                // Render widget access permissions
                object[,] accessNames = new object[3, 2];
                //accessNames[0, 0] = GetString("security.allusers");
                //accessNames[0, 1] = SecurityAccessEnum.AllUsers;
                accessNames[0, 0] = GetString("security.authenticated");
                accessNames[0, 1] = SecurityAccessEnum.AuthenticatedUsers;
                accessNames[1, 0] = GetString("security.globaladmin");
                accessNames[1, 1] = SecurityAccessEnum.GlobalAdmin;
                accessNames[2, 0] = GetString("security.authorizedroles");
                accessNames[2, 1] = SecurityAccessEnum.AuthorizedRoles;

                TableRow newRow = null;

                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);

                    // Generate cell holding access item name
                    newRow = new TableRow();
                    TableCell newCell = new TableCell();
                    newCell.CssClass = "matrix-header";
                    newCell.Text = accessNames[access, 0].ToString();
                    newRow.Cells.Add(newCell);

                    // Render the permissions access items
                    int permissionIndex = 0;
                    for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 1); permission++)
                    {
                        newCell = new TableCell();
                        newCell.CssClass = "matrix-cell";

                        int accessEnum = Convert.ToInt32(accessNames[access, 1]);
                        // Check if the currently processed access is applied for permission
                        bool isAllowed = CheckPermissionAccess(accessEnum, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);

                        // Disable column in roles grid if needed
                        if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                        {
                            gridMatrix.DisableColumn(permissionIndex);
                        }

                        // Insert the radio button for the current permission
                        var radio = new CMSRadioButton
                        {
                            Checked = isAllowed,
                            Enabled = Enable,
                        };
                        radio.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(this, permission + ";" + accessEnum));
                        newCell.Controls.Add(radio);

                        newRow.Cells.Add(newCell);
                        permissionIndex++;
                    }

                    tblMatrix.Rows.Add(newRow);
                }

                // Get permission matrix for roles of the current site/group            
                mNoRolesAvailable = !gridMatrix.HasData;
                if (!mNoRolesAvailable)
                {
                    lblRolesInfo.Visible = true;
                }
            }
        }
    }


    /// <summary>
    /// Indicates the permission acess.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>    
    private bool CheckPermissionAccess(int currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (WidgetInfo != null)
        {
            switch (currentPermission)
            {
                case 0:
                    result = ((int)WidgetInfo.AllowedFor == currentAccess);
                    break;
            }
        }

        // Make note about type of permission with access set to 'OnlyAuthorizedRoles'
        if (result && (currentAccess == 2))
        {
            onlyAuth[currentPermissionName] = true;
        }
        return result;
    }


    /// <summary>
    /// On item changed event.
    /// </summary>    
    private void gridMatrix_OnItemChanged(object sender, int roleId, int permissionId, bool allow)
    {
        if (!CheckPermissions("cms.widget", PERMISSION_MODIFY))
        {
            return;
        }

        if (allow)
        {
            WidgetRoleInfoProvider.AddRoleToWidget(roleId, WidgetID, permissionId);
        }
        else
        {
            WidgetRoleInfoProvider.RemoveRoleFromWidget(roleId, WidgetID, permissionId);
        }
    }


    #region "PostBack event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (!CheckPermissions("cms.widget", PERMISSION_MODIFY))
        {
            return;
        }

        string[] args = eventArgument.Split(';');

        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = Convert.ToInt32(args[0]);
            int access = Convert.ToInt32(args[1]);

            if (WidgetInfo != null)
            {
                // Update widget permission access information
                switch (permission)
                {
                    case 0:
                        WidgetInfo.AllowedFor = ((SecurityAccessEnum)access);
                        break;
                }

                // Save changes to the widget
                WidgetInfoProvider.SetWidgetInfo(WidgetInfo);
            }
        }
    }

    #endregion
}