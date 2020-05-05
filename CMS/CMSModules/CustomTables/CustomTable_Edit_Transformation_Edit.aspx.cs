using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "EditTransformation.General")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Transformation_Edit : CMSCustomTablesPage
{
    #region "Variables"

    private TransformationInfo mTransInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            if (mTransInfo == null)
            {
                mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("objectid", 0));
            }
            return mTransInfo;
        }
        set
        {
            mTransInfo = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        ucHierarchy.AddContentParameter(new UILayoutValue("ListPage", "~/CMSModules/CustomTables/CustomTable_Edit_Transformation_List.aspx"));
        ucHierarchy.AddContentParameter(new UILayoutValue("EditingPage", "CustomTable_Edit_Transformation_Edit.aspx"));
        ucHierarchy.AddContentParameter(new UILayoutValue("ParameterName", "objectid"));
        ucHierarchy.ShowPanelSeparator = true;

        if (TransInfo != null)
        {
            UIContext.EditedObject = TransInfo;
            ucHierarchy.PreviewURLSuffix = String.Format("&previewtransformationname={0}", TransInfo.TransformationFullName);
            ucHierarchy.PreviewObjectName = TransInfo.TransformationFullName;
        }

        base.OnInit(e);
    }

    #endregion
}