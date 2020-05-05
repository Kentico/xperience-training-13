using System;
using System.Data;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Permissions_Matrix : CMSRolesPage
{
    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "read" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Permissions", "Read"))
        {
            RedirectToAccessDenied("CMS.Permissions", "Read");
        }

        InitializeFilter();

        prmMatrix.GlobalRoles = ((SiteID <= 0) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin));
        prmMatrix.SiteID = SiteID;
        prmMatrix.RoleID = QueryHelper.GetInteger("roleid", 0);
        prmMatrix.OnDataLoaded += new CMSModules_Permissions_Controls_PermissionsMatrix.OnMatrixDataLoaded(prmMatrix_DataLoaded);
        prmMatrix.CornerText = GetString("Administration.Roles.Permission");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        prmMatrix.SelectedID = prmhdrHeader.SelectedID;
        if (prmMatrix.SelectedID != "0")
        {
            prmMatrix.SelectedType = prmhdrHeader.SelectedType;
        }
        prmMatrix.FilterChanged = prmhdrHeader.FilterChanged;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initialize permission filter control.
    /// </summary>
    private void InitializeFilter()
    {
        prmhdrHeader.SiteID = (SiteID <= 0) ? 0 : SiteID;
        prmhdrHeader.HideSiteSelector = ((SiteID <= 0) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin));
        prmhdrHeader.ShowUserSelector = false;
        prmhdrHeader.UseUniSelectorAutocomplete = false;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Replaces text in column header with localized string.
    /// </summary>
    /// <param name="ds">Data set to be processed</param>
    protected void prmMatrix_DataLoaded(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["RoleDisplayName"] = GetString("Administration.Roles.AllowPermission");
            }
        }
    }

    #endregion
}