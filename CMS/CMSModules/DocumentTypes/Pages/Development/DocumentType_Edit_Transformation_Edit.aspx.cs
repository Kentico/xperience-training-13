using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_Edit : CMSDeskPage
{
    #region "Variables"

    private TransformationInfo mTransInfo;
    private bool isDialog;

    #endregion


    #region "Properties"

    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            return mTransInfo ?? (mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("objectid", 0)) ?? TransformationInfoProvider.GetTransformation(QueryHelper.GetString("name", "")));
        }
        set
        {
            mTransInfo = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Page has been opened from CMSDesk
        isDialog = QueryHelper.GetBoolean("editonlycode", false);
        if (isDialog)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "objectid"))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }
        }
        else
        {
            CheckGlobalAdministrator();

            // Don't require site in Page types application
            RequireSite = false;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ucHierarchy.ShowPanelSeparator = true;

        if (TransInfo != null)
        {
            UIContext.EditedObject = TransInfo;
            Guid instanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
            ucHierarchy.PreviewURLSuffix = String.Format("&previewguid={0}&previewobjectidentifier={1}", instanceGUID, TransInfo.TransformationFullName);
            ucHierarchy.PreviewObjectName = TransInfo.TransformationFullName;
            ucHierarchy.DialogMode = isDialog;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (isDialog)
        {
            ScriptHelper.RegisterWOpenerScript(Page);
        }
    }

    #endregion
}
