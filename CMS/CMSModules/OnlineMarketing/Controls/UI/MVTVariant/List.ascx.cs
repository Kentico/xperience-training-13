using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_MVTVariant_List : CMSAdminListControl
{
    #region "Variables"

    private TreeNode mNode = null;
    private int mPageTemplateID = 0;
    private string mZoneID = string.Empty;
    private Guid mInstanceGUID = Guid.Empty;
    private int mNodeID = 0;
    private VariantTypeEnum mVariantType = VariantTypeEnum.Zone;
    private TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the current node.
    /// </summary>
    private TreeNode Node
    {
        get
        {
            if (mNode == null)
            {
                mNode = tree.SelectSingleNode(NodeID, LocalizationContext.PreferredCultureCode, tree.CombineWithDefaultCulture);
            }

            return mNode;
        }
        set
        {
            mNode = value;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
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
    /// Indicates if the control is used on the live site.
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
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Page template ID of the document which this MVT variants belongs to.
    /// </summary>
    public int PageTemplateID
    {
        get
        {
            return mPageTemplateID;
        }
        set
        {
            mPageTemplateID = value;
        }
    }


    /// <summary>
    /// Gets or sets the zone ID.
    /// </summary>
    public string ZoneID
    {
        get
        {
            return mZoneID;
        }
        set
        {
            mZoneID = value;
        }
    }


    /// <summary>
    /// Gets or sets the instance GUID. If the variant is a zone, then the InstanceGuid is Guid.Empty
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return mInstanceGUID;
        }
        set
        {
            mInstanceGUID = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of the variant (webPart/zone/widget).
    /// </summary>
    public VariantTypeEnum VariantType
    {
        get
        {
            return mVariantType;
        }
        set
        {
            mVariantType = value;
        }
    }


    /// <summary>
    /// NodeID of the current document. (Used for checking the access permissions).
    /// </summary>
    public int NodeID
    {
        get
        {
            return mNodeID;
        }
        set
        {
            mNodeID = value;
            mNode = null;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.EditActionUrl = "Edit.aspx?variantid={0}&nodeid=" + NodeID;

        // Grid initialization                
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);

        // Build where condition
        string where = "MVTVariantPageTemplateID = " + PageTemplateID;

        // Display only variants for the current document
        if (Node != null)
        {
            where = SqlHelper.AddWhereCondition(where, "(MVTVariantDocumentID = " + Node.DocumentID + ") OR MVTVariantDocumentID IS NULL");
        }

        // Display variants just for a specific web part
        if (InstanceGUID != Guid.Empty)
        {
            where = SqlHelper.AddWhereCondition(where, "MVTVariantInstanceGUID = '" + InstanceGUID + "'");
        }
        // Display variants for zone
        else if (!string.IsNullOrEmpty(ZoneID))
        {
            where = SqlHelper.AddWhereCondition(where, "MVTVariantZoneID = '" + SqlHelper.EscapeQuotes(ZoneID) + "'");
        }

        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, where);
    }


    /// <summary>
    /// Handles UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action which should be performed</param>
    /// <param name="actionArgument">ID of the item the action should be performed with</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int mvtvariantId = ValidationHelper.GetInteger(actionArgument, 0);
        if (mvtvariantId > 0)
        {
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    if (CheckPermissions("CMS.MVTest", PERMISSION_MANAGE))
                    {
                        // Get the web part instance Guid in order to clear the webpart's cache
                        Guid webPartInstanceGuid = Guid.Empty;
                        MVTVariantInfo vi = MVTVariantInfoProvider.GetMVTVariantInfo(mvtvariantId);

                        // Delete the object
                        MVTVariantInfoProvider.DeleteMVTVariantInfo(mvtvariantId);
                        RaiseOnDelete();

                        // Log widget variant synchronization
                        if ((vi != null) && (vi.MVTVariantDocumentID > 0))
                        {
                            // Log synchronization
                            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, tree);
                        }
                    }
                    break;
            }
        }
    }
    
    #endregion
}