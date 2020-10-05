using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_FormControls_Roles_SecurityAddRoles : ReadOnlyFormEngineUserControl
{
    #region "Private variables"

    private int mNodeID;
    private TreeNode mNode;
    private TreeProvider mTree;

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree provider.
    /// </summary>
    protected TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(MembershipContext.AuthenticatedUser));
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets node id.
    /// </summary>
    public int NodeID
    {
        get
        {
            return mNodeID;
        }
        set
        {
            // Clear TreeNode on id change
            if (mNodeID != value)
            {
                mNode = null;
            }

            mNodeID = value;
        }
    }


    /// <summary>
    /// Gets or sets the TreeNode.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            if ((mNode == null) && (NodeID > 0))
            {
                // Get node
                mNode = Tree.SelectSingleNode(NodeID);
            }
            return mNode;
        }
        set
        {
            mNode = value;
            // Update NodeID
            if (mNode != null)
            {
                mNodeID = mNode.NodeID;
            }
        }
    }


    /// <summary>
    /// Gets or sets bizform id.
    /// </summary>
    public int FormID { get; set; }


    /// <summary>
    /// Enables or disables the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return usRoles.Enabled;
        }
        set
        {
            usRoles.Enabled = value;
        }
    }


    /// <summary>
    /// Returns current uniselector.
    /// </summary>
    public UniSelector CurrentSelector => usRoles;


    /// <summary>
    /// Gets or sets subscriber.
    /// </summary>
    public string CurrentValues { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets if live site property.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            usRoles.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter is should be shown.
    /// </summary>
    public bool ShowSiteFilter { get; set; } = true;

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Add event handling
        usRoles.OnItemsSelected += usRoles_OnItemsSelected;

        // Check if site filter should be displayed
        if (ShowSiteFilter)
        {
            usRoles.SetValue("DefaultFilterValue", SiteContext.CurrentSiteID);
            usRoles.SetValue("FilterMode", "role");
            usRoles.SetValue("ShowSiteFilter", true);
            usRoles.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
        }
        else
        {
            usRoles.WhereCondition = SqlHelper.AddWhereCondition(usRoles.WhereCondition, "(SiteID IS NULL OR SiteID = " + SiteContext.CurrentSiteID + ")");
        }

        // Check node permissions
        if (Node != null)
        {
            // Check if filter should be used
            if (ShowSiteFilter)
            {
                // Add sites filter
                usRoles.FilterControl = "~/CMSFormControls/Filters/SiteGroupFilter.ascx";
                usRoles.SetValue("FilterMode", "role");
            }

            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) != AuthorizationResultEnum.Allowed)
            {
                usRoles.Enabled = false;
                return;
            }
        }

        // Check bizform 'EditForm' permission
        if (FormID > 0)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                usRoles.Enabled = false;
                return;
            }
        }

        if (!IsLiveSite)
        {
            ScriptHelper.RegisterDialogScript(Page);
        }
    }


    /// <summary>
    /// Reloads control data.
    /// </summary>
    public void ReloadData()
    {
        usRoles.Reload(true);
    }


    /// <summary>
    /// On items selected event handling.
    /// </summary>    
    private void usRoles_OnItemsSelected(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usRoles.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, CurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                int roleID = ValidationHelper.GetInteger(item, 0);

                if (FormID > 0)
                {
                    // Remove role from form
                    BizFormRoleInfo.Provider.Remove(roleID, FormID);
                }
                else if (Node != null)
                {
                    RoleInfo ri = RoleInfo.Provider.Get(roleID);
                    // Remove role from treenode
                    AclItemInfoProvider.RemoveRole(NodeID, ri);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(CurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                int roleID = ValidationHelper.GetInteger(item, 0);

                if (FormID > 0)
                {
                    // Add BizForm role
                    BizFormRoleInfo.Provider.Add(roleID, FormID);
                }
                else if (Node != null)
                {
                    RoleInfo ri = RoleInfo.Provider.Get(roleID);
                    // Add role to treenode
                    AclItemInfoProvider.SetRolePermissions(Node, 0, 0, ri);
                }
            }
        }

        // Log synchronization task
        if (Node != null)
        {
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, Node.TreeProvider);
        }

        RaiseOnChanged();
    }

    #endregion
}