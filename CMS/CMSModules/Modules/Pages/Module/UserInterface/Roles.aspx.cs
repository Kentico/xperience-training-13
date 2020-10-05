using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface.Roles")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_Roles : GlobalAdminPage
{
    #region "Constants"

    /// <summary>
    /// CSS class for highlighted UI permission matrix rows.
    /// </summary>
    private const string HIGHLIGHTED_ROW_CSS = "highlighted";

    #endregion


    #region "Variables"

    private int mElementID;
    private UserInfo mSelectedUser;
    private UIElementInfo mElement;
    private ResourceInfo mElementResource;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets selected user ID.
    /// </summary>
    private int SelectedUserID
    {
        get
        {
            return ValidationHelper.GetInteger(userSelector.Value, 0);
        }
    }


    /// <summary>
    /// Gets UserInfo object for selected user.
    /// </summary>
    private UserInfo SelectedUser
    {
        get
        {
            if ((mSelectedUser == null) && (SelectedUserID > 0))
            {
                mSelectedUser = UserInfo.Provider.Get(SelectedUserID);
            }

            return mSelectedUser;
        }
    }


    /// <summary>
    /// Gets selected UI element ID.
    /// </summary>
    private int ElementID
    {
        get
        {
            if (mElementID == 0)
            {
                mElementID = QueryHelper.GetInteger("elementid", 0);
            }
            return mElementID;
        }
    }


    /// <summary>
    /// Gets UIElementInfo object for selected UI element.
    /// </summary>
    private UIElementInfo Element
    {
        get
        {
            return mElement ?? (mElement = UIElementInfo.Provider.Get(ElementID));
        }
    }


    /// <summary>
    /// Gets element resource for selected UI element.
    /// </summary>
    private ResourceInfo ElementResource
    {
        get
        {
            if (mElementResource == null)
            {
                if (Element != null)
                {
                    mElementResource = ResourceInfo.Provider.Get(Element.ElementResourceID);
                }
            }
            return mElementResource;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        bool enabled = true;
        if (Element != null)
        {
            enabled = (QueryHelper.GetInteger("moduleId", 0) == Element.ElementResourceID) || !UIElementInfoProvider.AllowEditOnlyCurrentModule;
            gridMatrix.Enabled = enabled;
        }

        // Initialize UI permission matrix
        gridMatrix.ColumnsCount = 1;
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.ContentBeforeRowCssClass = "content-before";
        gridMatrix.OnGetRowItemCssClass += gridMatrix_OnGetRowItemCssClass;
        gridMatrix.NoRecordsMessage = GetString("general.emptymatrix");

        // Show site selector placeholder on locations where the site selector can be shown
        CurrentMaster.DisplaySiteSelectorPanel = true;

        //Initialize site selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_SelectedIndexChanged;

        // Initialize user selector
        userSelector.SiteID = (siteSelector.SiteID > 0) ? siteSelector.SiteID : 0;
        userSelector.DropDownSingleSelect.AutoPostBack = true;

        // Display all users if global
        userSelector.DisplayUsersFromAllSites = userSelector.SiteID <= 0;

        chkUserOnly.Text = GetString("Administration-Permissions_Header.UserRoles");

        if ((Element != null) && Element.ElementLevel > 0)
        {
            // Prepare header action
            HeaderActions actions = CurrentMaster.HeaderActions;

            // Create 'Copy from parent' action
            actions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("uiprofile.permissionsfromparent"),
                OnClientClick = "return ConfirmCopyFromParent();",
                CommandName = "copyFromParent",
                CommandArgument = string.Empty,
                Enabled = enabled
            });

            actions.ActionPerformed += HeaderActions_ActionPerformed;

            // Register javascript to confirm copy 
            string script = "function ConfirmCopyFromParent() {return confirm(" + ScriptHelper.GetString(GetString("uiprofile.ConfirmCopyFromParent")) + ");}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmCopyFromParent", ScriptHelper.GetScript(script));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Load the matrix
        if (Element != null)
        {
            // Disable check box if no user selected
            if (SelectedUserID > 0)
            {
                chkUserOnly.Enabled = true;
            }
            else
            {
                chkUserOnly.Checked = false;
                chkUserOnly.Enabled = false;
            }

            // Set info label
            headTitle.Text = String.Format(GetString("resource.ui.rolesinfo"), HTMLHelper.HTMLEncode(Element.ElementDisplayName));

            // Set matrix parameters
            gridMatrix.QueryParameters = GetQueryParameters(siteSelector.SiteID, Element.ElementDisplayName);
            gridMatrix.WhereCondition = GetWhereCondition();
            gridMatrix.ShowContentBeforeRow = (SelectedUser != null);

            // Get content before rows
            GenerateBeforeRowsContent();

            ucDisabledModule.SiteName = siteSelector.SiteName;
            ucDisabledModule.InfoText = GetString("resource.ui.disabled");

            ShowInformation(String.Empty);

            if (!gridMatrix.HasData)
            {
                plcUpdate.Visible = false;
                headTitle.Text = (chkUserOnly.Checked) ? GetString("general.norolemember") : GetString("uiprofile.norole");
            }
            else
            {
                // Inform user that global admin was selected
                if ((SelectedUserID > 0) && (SelectedUser != null) && (SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
                {
                    ShowInformation(GetString("uiprofile.GlobalAdministrator"));
                }

                plcUpdate.Visible = true;
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    protected void UniSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Element != null)
        {
            // Reset selected user
            userSelector.ReloadData();
            mSelectedUser = null;

            // Set matrix current page to first
            gridMatrix.Pager.CurrentPage = 1;
        }
    }


    protected void gridMatrix_OnItemChanged(object sender, int rowItemId, int colItemId, bool newState)
    {
        if (newState)
        {
            RoleUIElementInfo.Provider.Add(rowItemId, colItemId);
        }
        else
        {
            RoleUIElementInfo.Provider.Remove(rowItemId, colItemId);
        }

        // Invalidate all users
        UserInfo.TYPEINFO.InvalidateAllObjects();

        // Forget old user
        CurrentUser = null;

        // Clear hashtables with users
        ProviderHelper.ClearHashtables(UserInfo.OBJECT_TYPE, true);

        // Update content before rows
        GenerateBeforeRowsContent();
    }


    protected string gridMatrix_OnGetRowItemCssClass(object sender, DataRow dr)
    {
        string roleName = ValidationHelper.GetString(dr["RoleName"], String.Empty);

        // Check if all necessary data are available
        if (!String.IsNullOrEmpty(roleName) && (SelectedUser != null))
        {
            if (SelectedUser.IsInRole(roleName, siteSelector.SiteName, (siteSelector.SiteID <= 0), true))
            {
                return HIGHLIGHTED_ROW_CSS;
            }
        }

        return String.Empty;
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "copyfromparent":
                if (Element != null)
                {
                    CopyFromParent(Element);

                    // Set matrix current page to first
                    gridMatrix.Pager.CurrentPage = 1;
                }
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns query parameters for permission matrix.
    /// </summary>
    /// <param name="siteId">Site ID</param>
    /// <param name="elementName">Element display name</param>
    private QueryDataParameters GetQueryParameters(int siteId, string elementName)
    {
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@SiteID", (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue) ? 0 : siteId);
        parameters.Add("@ElementID", mElementID);
        parameters.Add("@ElementDisplayName", ResHelper.LocalizeString(elementName));

        return parameters;
    }


    /// <summary>
    /// Copies role binding from parent UI element.
    /// </summary>
    /// <param name="element">Element which are permissions copied to</param>
    private void CopyFromParent(UIElementInfo element)
    {
        using (var tr = new CMSTransactionScope())
        {
            if (element != null)
            {
                // Delete existing bindings
                DataSet elemRoles = RoleUIElementInfo.Provider.Get().WhereEquals("ElementID", element.ElementID);
                if (!DataHelper.DataSourceIsEmpty(elemRoles))
                {
                    foreach (DataRow dr in elemRoles.Tables[0].Rows)
                    {
                        // Get role id
                        int roleId = ValidationHelper.GetInteger(dr["RoleID"], 0);
                        // Remove binding
                        RoleUIElementInfo.Provider.Remove(roleId, element.ElementID);
                    }
                }

                // Add same bindings as parent has
                int parentElemId = element.ElementParentID;

                DataSet parentRoles = RoleUIElementInfo.Provider.Get().WhereEquals("ElementID", parentElemId);
                if (!DataHelper.DataSourceIsEmpty(parentRoles))
                {
                    foreach (DataRow dr in parentRoles.Tables[0].Rows)
                    {
                        // Get role id
                        int roleId = ValidationHelper.GetInteger(dr["RoleID"], 0);
                        // Create binding
                        RoleUIElementInfo.Provider.Add(roleId, element.ElementID);
                    }
                }
            }

            // Commit transaction
            tr.Commit();
        }

        // Invalidate all users
        UserInfo.TYPEINFO.InvalidateAllObjects();

        // Clear hashtables with users
        ProviderHelper.ClearHashtables(UserInfo.OBJECT_TYPE, true);
    }


    /// <summary>
    /// Gets user effective UI permission HTML content.
    /// </summary>
    private void GenerateBeforeRowsContent()
    {
        // Check if every necessary property is set
        if ((SelectedUser != null) && (Element != null) && (ElementResource != null))
        {
            // Initialize variables used during rendering
            string userName = HTMLHelper.HTMLEncode(TextHelper.LimitLength(Functions.GetFormattedUserName(SelectedUser.UserName, SelectedUser.FullName), 50));
            bool authorizedToUIElement = UserInfoProvider.IsAuthorizedPerUIElement(ElementResource.ResourceName,
                                                      new[] { Element.ElementName },
                                                      siteSelector.SiteName,
                                                      SelectedUser,
                                                      true,
                                                      siteSelector.SiteID <= 0);
            // Create header table cell
            var tcHeader = new TableCell
            {
                CssClass = "matrix-header",
                ToolTip = userName,
                Text = userName
            };

            var tr = gridMatrix.ContentBeforeRow;
            tr.Cells.Add(tcHeader);

            // Create UI permission cell
            var tc = new TableCell();
            var chk = new CMSCheckBox
            {
                Checked = SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || authorizedToUIElement,
                Enabled = false,
                ToolTip = GetString("Administration-Permissions_Matrix.NotAdjustable")
            };

            tc.Controls.Add(chk);
            tr.Cells.Add(tc);

            UniMatrix.AddFillingCell<TableCell>(tr);
        }
    }


    /// <summary>
    /// Gets where condition for the matrix.
    /// </summary>
    /// <returns>String representing where condition for the matrix</returns>
    private string GetWhereCondition()
    {
        string where = null;

        if (chkUserOnly.Checked && (SelectedUserID > 0))
        {
            // Get selected site name
            string siteName = UserInfo.GLOBAL_ROLES_KEY;
            if (siteSelector.SiteID > 0)
            {
                siteName = siteSelector.SiteName.ToLowerCSafe();
            }

            string rolesWhere = SelectedUser.GetRoleIdList((siteSelector.SiteID <= 0), true, siteName);

            // Add roles where condition
            if (!String.IsNullOrEmpty(rolesWhere))
            {
                where = "RoleID IN (" + rolesWhere + ")";
            }
            else
            {
                where = SqlHelper.NO_DATA_WHERE;
                gridMatrix.StopProcessing = true;
            }
        }

        return where;
    }

    #endregion
}
