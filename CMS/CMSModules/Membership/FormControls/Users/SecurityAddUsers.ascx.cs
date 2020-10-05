using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_FormControls_Users_SecurityAddUsers : ReadOnlyFormEngineUserControl
{
    #region "Private variables"

    private int mNodeID = 0;
    private TreeNode mNode = null;
    private string mCurrentValues = String.Empty;
    private TreeProvider mTree = null;

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
    /// Returns current uniselector.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            return usUsers.UniSelector;
        }
    }

    /// <summary>
    /// Gets or sets subscriber.
    /// </summary>
    public string CurrentValues
    {
        get
        {
            return mCurrentValues;
        }
        set
        {
            mCurrentValues = value;
        }
    }


    /// <summary>
    /// Enables or disables the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return usUsers.Enabled;
        }
        set
        {
            usUsers.Enabled = value;
        }
    }


    /// <summary>
    /// Enables or disables site filter in uni selector.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return usUsers.ShowSiteFilter;
        }
        set
        {
            usUsers.ShowSiteFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the site. Only users of this site are shown in selector.
    /// Note. SiteID is not used if site filter is enabled
    /// </summary>
    public int SiteID
    {
        get
        {
            return usUsers.SiteID;
        }
        set
        {
            usUsers.SiteID = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
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
            usUsers.IsLiveSite = value;
        }
    }

    #endregion


    #region "Protected properties"

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


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Add sites filter        
        usUsers.UniSelector.SetValue("FilterMode", "user");
        usUsers.ResourcePrefix = "addusers";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Add event handling
        usUsers.UniSelector.OnItemsSelected += usUsers_OnItemsSelected;
        usUsers.UniSelector.OnSelectionChanged += usUsers_OnItemsSelected;

        // Check node permissions
        if (Node != null)
        {
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) != AuthorizationResultEnum.Allowed)
            {
                usUsers.Enabled = false;
                return;
            }
        }
    }


    protected void usUsers_OnItemsSelected(object sender, EventArgs e)
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usUsers.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, CurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int userID = ValidationHelper.GetInteger(item, 0);

                    if (Node != null)
                    {
                        UserInfo ui = UserInfo.Provider.Get(userID);
                        if (ui != null)
                        {
                            // Remove user from treenode
                            AclItemInfoProvider.RemoveUser(NodeID, ui);
                        }
                    }
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(CurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int userID = ValidationHelper.GetInteger(item, 0);

                    if (Node != null)
                    {
                        UserInfo ui = UserInfo.Provider.Get(userID);
                        if (ui != null)
                        {
                            // Remove user from treenode
                            AclItemInfoProvider.SetUserPermissions(Node, 0, 0, ui);
                        }
                    }
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


    /// <summary>
    /// Reloads the data of the UniSelector.
    /// </summary>
    public void ReloadData()
    {
        usUsers.Value = CurrentValues;
        usUsers.ReloadData();
    }
}