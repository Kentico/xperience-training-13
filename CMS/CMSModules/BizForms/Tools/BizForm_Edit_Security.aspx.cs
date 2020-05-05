using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("cms.form", "Forms.Security")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_Security : CMSBizFormPage
{
    #region "Variables"

    private BizFormInfo formInfo;

    #endregion


    #region "Properties"

    protected BizFormInfo FormInfo
    {
        get
        {
            return formInfo ?? (formInfo = EditedObject as BizFormInfo);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        // Control initialization
        addRoles.FormID = FormInfo.FormID;
        addRoles.CurrentSelector.IsLiveSite = false;
        addRoles.Changed += addRoles_Changed;
        addRoles.ShowSiteFilter = false;

        if (!RequestHelper.IsPostBack() && (FormInfo != null))
        {
            // Load data
            radAllUsers.Checked = (FormInfo.FormAccess == FormAccessEnum.AllBizFormUsers);
            radOnlyRoles.Checked = !radAllUsers.Checked;

            // Load list with allowed roles
            LoadRoles();
        }
        else
        {
            if (addRoles.CurrentSelector.Enabled)
            {
                DataSet ds = BizFormInfoProvider.GetFormAuthorizedRoles(FormInfo.FormID);
                addRoles.CurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "RoleID"));
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable add/remove for unauthorized users
        bool authorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm");

        addRoles.CurrentSelector.Enabled = radOnlyRoles.Checked && authorized;
        btnRemoveRole.Enabled = radOnlyRoles.Checked && authorized;
        lstRoles.Enabled = radOnlyRoles.Checked;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// On Add roles changed event.
    /// </summary>
    private void addRoles_Changed(object sender, EventArgs e)
    {
        LoadRoles();

        // Clear authorized roles from hashtable
        FormInfo.ClearAuthorizedRoles();

        pnlUpdate.Update();
    }


    protected void btnRemoveRole_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }

        foreach (ListItem item in lstRoles.GetSelectedItems())
        {
            // Remove role-form association from database
            BizFormRoleInfoProvider.DeleteBizFormRoleInfo(Convert.ToInt32(item.Value), FormInfo.FormID);
        }

        LoadRoles();

        // Clear authorized roles from hashtable
        FormInfo.ClearAuthorizedRoles();
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }

        if (FormInfo != null)
        {
            if (radAllUsers.Checked)
            {
                FormInfo.FormAccess = FormAccessEnum.AllBizFormUsers;
                BizFormRoleInfoProvider.RemoveAllRolesFromForm(FormInfo.FormID);

                // Clear authorized roles from hashtable
                FormInfo.ClearAuthorizedRoles();
                lstRoles.Items.Clear();
            }
            else
            {
                FormInfo.FormAccess = FormAccessEnum.OnlyAuthorizedRoles;
            }

            BizFormInfoProvider.SetBizFormInfo(FormInfo);

            ShowChangesSaved();
        }
    }


    protected void radOnlyRoles_CheckedChanged(object sender, EventArgs e)
    {
        LoadRoles();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads list of roles authorized for form access.
    /// </summary>
    protected void LoadRoles()
    {
        DataSet ds = BizFormInfoProvider.GetFormAuthorizedRoles(FormInfo.FormID);
        addRoles.CurrentSelector.Value = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "RoleID"));

        lstRoles.Items.Clear();
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            string name = Convert.ToString(dr["RoleDisplayName"]);
            if (ValidationHelper.GetInteger(dr["SiteID"], 0) == 0)
            {
                name += " " + GetString("general.global");
            }
            lstRoles.Items.Add(new ListItem(name, Convert.ToString(dr["RoleID"])));
        }
    }

    #endregion
}