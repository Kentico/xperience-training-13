using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Controls_HierarchicalTransformations_List : CMSAdminEditControl
{
    #region "Variables"

    private TransformationInfo mTransInfo;
    private bool mDialogMode;
    private string mTemplateType;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected template type.
    /// </summary>
    public string TemplateType
    {
        get
        {
            return mTemplateType;
        }
        set
        {
            mTemplateType = value;
        }
    }


    /// <summary>
    /// Transformation info.
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            return mTransInfo;
        }
        set
        {
            mTransInfo = value;
        }
    }


    /// <summary>
    /// Indicates whether control is shown in modal dialog window (different master page).
    /// </summary>
    public bool DialogMode
    {
        get
        {
            return mDialogMode;
        }
        set
        {
            mDialogMode = value;
        }
    }


    /// <summary>
    /// Indicates if control is placed in Site manager.
    /// </summary>
    public static bool IsSiteManager
    {
        get
        {
            return QueryHelper.GetBoolean("issitemanager", false) && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ugTransformations.GridName = "~/CMSModules/DocumentTypes/Controls/HierarchicalTransformations_List.xml";
        ugTransformations.OnLoadColumns += ugTransformations_OnLoadColumns;
        ugTransformations.OnExternalDataBound += ugTransformations_OnExternalDataBound;
        ugTransformations.OnAction += ugTransformations_OnAction;
        ugTransformations.OrderBy = "ClassName";

        ugTransformations.OnDataReload += ugTransformations_OnDataReload;
        ugTransformations.ShowActionsMenu = true;

        // All column names retrievable from XML
        ugTransformations.AllColumns = "ID, Level, Type, ClassName, TransformationName";
    }


    private DataSet ugTransformations_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        //Set filters
        int level = ValidationHelper.GetInteger(txtLevel.Text, -1);
        string docType = txtDocTypes.Text;

        //Set new info to XML collection
        HierarchicalTransformations transf = LoadTransformation();

        int pageSize = ValidationHelper.GetInteger(ugTransformations.PageSizeDropdown.SelectedValue, 0);
        int count = transf.ItemsCount;

        // Hide filter when no pager used for grid or if all items count is larger then page size
        if ((pageSize == TreeProvider.ALL_LEVELS) || (count < pageSize))
        {
            pnlFilter.Visible = false;
        }
        else
        {
            pnlFilter.Visible = true;
        }

        return transf.GetDataSet(level, HierarchicalTransformations.StringToUniViewItemType(TemplateType), docType);
    }


    private void ugTransformations_OnLoadColumns()
    {
        if (TemplateType == "all")
        {
            ugTransformations.AddColumn(GetString("general.type"), "Type", 10, true);
        }
    }


    protected object ugTransformations_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "level":
                int level = (int)parameter;
                if (level == -1)
                {
                    return "All";
                }
                break;

            case "doctype":
                string docType = (string)parameter;
                if (docType == String.Empty)
                {
                    return "All";
                }
                break;
        }
        return parameter;
    }


    /// <summary>
    /// Load transformation for xml usage.
    /// </summary>
    private HierarchicalTransformations LoadTransformation()
    {
        HierarchicalTransformations transformations = new HierarchicalTransformations("ClassName");
        if (TransInfo != null)
        {
            if (!String.IsNullOrEmpty(TransInfo.TransformationHierarchicalXML))
            {
                transformations.LoadFromXML(TransInfo.TransformationHierarchicalXML);
            }
        }
        return transformations;
    }


    protected void ugTransformations_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            string isManager = IsSiteManager ? "&sitemanager=true" : String.Empty;
            
            URLHelper.Redirect(UrlResolver.ResolveUrl(String.Format(
                "HierarchicalTransformations_Transformations_Edit.aspx?guid={0}&objectid={1}&templatetype={2}&editonlycode={3}&tabmode={4}&instanceguid={5}&aliaspath={6}{7}",
                actionArgument, 
                TransInfo.TransformationID, 
                TemplateType, 
                mDialogMode, 
                QueryHelper.GetInteger("tabmode", 0), 
                QueryHelper.GetGuid("instanceguid", Guid.Empty), 
                QueryHelper.GetString("aliaspath", ""), isManager)
            ));
        }
        if (actionName == "delete")
        {
            HierarchicalTransformations transf = LoadTransformation();
            transf.DeleteTransformation(new Guid(Convert.ToString(actionArgument)));
            TransInfo.TransformationHierarchicalXML = transf.GetXML();
            TransformationInfoProvider.SetTransformation(TransInfo);

            //Reloads data
            ugTransformations.ReloadData();
        }
    }

    #endregion
}