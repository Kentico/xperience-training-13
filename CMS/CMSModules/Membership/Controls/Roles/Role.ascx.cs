using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI.Configuration;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Roles_Role : CMSAdminEditControl
{
    #region "Public properties"

    public int SelectedTab
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["selectedtab"], 0);
        }
        set
        {
            ViewState["selectedtab"] = (object)value;
        }
    }


    public int SiteID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["siteid"], -1);
        }
        set
        {
            ViewState["siteid"] = (object)value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region "security"

        RoleEdit.OnCheckPermissions += new CheckPermissionsEventHandler(RoleEdit_OnCheckPermissions);
        RoleUsers.OnCheckPermissions += new CheckPermissionsEventHandler(RoleUsers_OnCheckPermissions);

        #endregion


        ltlScript.Text = ScriptHelper.GetScript("function UpdateForm(){ " + Page.ClientScript.GetPostBackEventReference(btnUpdate, "") + "; } \n");

        // Menu initialization
        tabMenu.UrlTarget = "_self";        
        tabMenu.TabItems.Add(new TabItem(){ Text= GetString("general.general")});
        tabMenu.TabItems.Add(new TabItem(){ Text= GetString("general.users")});        
        tabMenu.UsePostback = true;
        tabMenu.UseClientScript = true;
        tabMenu.OnTabClicked += new EventHandler(tabMenu_OnTabChanged);
        tabMenu.TabControlIdPrefix = ClientID;

        btnUpdate.Attributes.Add("style", "display:none;");


        ReloadData(false);
    }


    #region "Security handlers"

    private void RoleUsers_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void RoleEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// Reloads and displays appropriate controls.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        RoleEdit.ItemID = RoleUsers.RoleID = ItemID;
        RoleEdit.SiteID = SiteID;
        RoleEdit.DisplayMode = DisplayMode;
        RoleEdit.ReloadData(forceReload);

        RoleEdit.Visible = false;
        RoleUsers.Visible = false;

        tabMenu.SelectedTab = SelectedTab;

        switch (SelectedTab)
        {
            case 0:
            default:
                RoleEdit.Visible = true;
                break;

            case 1:
                RoleUsers.Visible = true;
                RoleUsers.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Tab change event handler.
    /// </summary>
    private void tabMenu_OnTabChanged(object sender, EventArgs e)
    {
        SelectedTab = tabMenu.SelectedTab;
        ReloadData(false);
    }


    /// <summary>
    /// This function is executed by callback iniciated by 'Select roles' dialog after its closing.
    /// </summary>
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        ReloadData(false);
    }
}
