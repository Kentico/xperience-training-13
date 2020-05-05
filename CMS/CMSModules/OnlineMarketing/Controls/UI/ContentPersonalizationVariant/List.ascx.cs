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


public partial class CMSModules_OnlineMarketing_Controls_UI_ContentPersonalizationVariant_List : CMSAdminListControl
{
    #region "Variables"

    private TreeNode mNode = null;
    private int mNodeID = 0;
    private int mPageTemplateID = 0;
    private string mZoneID = string.Empty;
    private Guid mInstanceGUID = Guid.Empty;
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


    /// <summary>
    /// Page template ID of the document which this variants belong to.
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
    /// Gets or sets a value indicating whether sorting of the unigrid is allowed.
    /// </summary>
    public bool AllowSorting
    {
        get
        {
            return gridElem.GridView.AllowSorting;
        }
        set
        {
            gridElem.GridView.AllowSorting = value;
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
        gridElem.ZeroRowsText = GetString("contentvariant.nodata");

        // Grid initialization                
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);

        // Build where condition
        var where = new WhereCondition().WhereEquals("VariantPageTemplateID", PageTemplateID);

        // Display only variants for the current document
        if (Node != null)
        {
            where.WhereEqualsOrNull("VariantDocumentID", Node.DocumentID);
        }

        // Display variants just for a specific zone/webpart/widget
        if (!string.IsNullOrEmpty(ZoneID))
        {
            where.WhereEquals("VariantZoneID", ZoneID);

            if (InstanceGUID != Guid.Empty)
            {
                // Web part/widget condition
                where.WhereEquals("VariantInstanceGUID", InstanceGUID );
            }
        }

        gridElem.WhereCondition = where.ToString(expand: true);
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblError.Visible = !string.IsNullOrEmpty(lblError.Text);
    }


    /// <summary>
    /// Handles UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action which should be performed</param>
    /// <param name="actionArgument">ID of the item the action should be performed with</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (!CheckPermissions("CMS.ContentPersonalization", "Manage"))
        {
            return;
        }

        int variantId = ValidationHelper.GetInteger(actionArgument, 0);
        if (variantId > 0)
        {
            string action = actionName.ToLowerCSafe();
            switch (action)
            {
                case "delete":
                    {
                        // Get the instance in order to clear the cache
                        ContentPersonalizationVariantInfo vi = ContentPersonalizationVariantInfoProvider.GetContentPersonalizationVariant(variantId);

                        // Delete the object
                        ContentPersonalizationVariantInfoProvider.DeleteContentPersonalizationVariant(variantId);
                        RaiseOnAction(string.Empty, null);

                        // Log widget variant synchronization
                        if ((vi != null) && (vi.VariantDocumentID > 0))
                        {
                            // Log synchronization
                            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, tree);
                        }
                    }
                    break;

                case "up":
                case "down":
                    {
                        // Get the instance in order to clear the cache
                        ContentPersonalizationVariantInfo vi = ContentPersonalizationVariantInfoProvider.GetContentPersonalizationVariant(variantId);

                        // Use try/catch due to license check
                        try
                        {
                            if (action == "up")
                            {
                                // Move up
                                ContentPersonalizationVariantInfoProvider.MoveVariantUp(variantId);
                            }
                            else
                            {
                                // Move down
                                ContentPersonalizationVariantInfoProvider.MoveVariantDown(variantId);
                            }

                            RaiseOnAction(string.Empty, null);

                            // Log widget variant synchronization
                            if (vi.VariantDocumentID > 0)
                            {
                                // Log synchronization
                                DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, tree);
                            }
                        }
                        catch (Exception ex)
                        {
                            lblError.Visible = true;
                            lblError.Text = ex.Message;
                        }
                    }
                    break;
            }
        }
    }

    #endregion
}