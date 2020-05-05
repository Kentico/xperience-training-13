using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_FormControls_Documents_ChangePermissions_ChangePermissions : CMSModalPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register save changes
        ScriptHelper.RegisterSaveChanges(Page);

        // Set master page
        PageTitle.TitleText = GetString("selectsinglepath.setpermissions");
        if (QueryHelper.ValidateHash("hash"))
        {
            if (NodeID == 0)
            {
                lblInfo.Text = GetString("content.documentnotexists");
                securityElem.Visible = false;
            }
            else
            {
                // Setup security control
                securityElem.DisplayButtons = false;
                securityElem.NodeID = NodeID;
            }
        }
        else
        {
            securityElem.Visible = false;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(lblInfo.Text))
        {
            lblInfo.Visible = true;
        }
    }

    #endregion


    protected void btnSave_OnClick(object sender, EventArgs e)
    {
        securityElem.Save();
    }
}
