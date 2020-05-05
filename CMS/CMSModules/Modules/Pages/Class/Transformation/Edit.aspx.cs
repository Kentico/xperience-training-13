using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Modules_Pages_Class_Transformation_Edit : GlobalAdminPage
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
                mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("transformationid", 0));
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
        ucHierarchy.AddContentParameter(new UILayoutValue("ListPage", "~/CMSModules/Modules/Pages/Class/List.aspx"));
        ucHierarchy.AddContentParameter(new UILayoutValue("EditingPage", "Edit.aspx"));
        ucHierarchy.AddContentParameter(new UILayoutValue("ParameterName", "classid"));
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