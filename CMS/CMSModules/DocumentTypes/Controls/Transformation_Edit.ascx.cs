using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Controls_Transformation_Edit : CMSAdminEditControl
{
    #region "Variables"

    private TransformationInfo mTransInfo;

    #endregion


    #region "Public Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ID of control's document type.
    /// </summary>
    public int DocumentTypeID
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation info object.
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            return mTransInfo ?? (mTransInfo = new TransformationInfo());
        }
        set
        {
            mTransInfo = value;
            EditedObject = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        rfvCodeName.ErrorMessage = GetString("general.erroridentifierformat");
        if (!RequestHelper.IsPostBack())
        {
            txtName.Text = TransInfo.TransformationName;
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        string codeName = txtName.Text.Trim();
        //Test if codename not empty
        string errorMessage = new Validator().NotEmpty(codeName, rfvCodeName.ErrorMessage).Result;

        //Test right format
        if ((errorMessage == "") && (!ValidationHelper.IsIdentifier(codeName.Trim())))
        {
            errorMessage = GetString("general.erroridentifierformat");
        }

        if (errorMessage != String.Empty)
        {
            ShowError(errorMessage);
            return;
        }

        TransInfo.TransformationName = txtName.Text;

        //If edit no DocumentTypeID is set
        if (DocumentTypeID != 0)
        {
            TransInfo.TransformationClassID = DocumentTypeID;
        }

        //Save new Transformation
        TransformationInfo ti = TransformationInfoProvider.GetTransformation(TransInfo.TransformationFullName);
        if ((ti != null) && (ti.TransformationID != TransInfo.TransformationID))
        {
            ShowError(GetString("DocumentType_Edit_Transformation_Edit.UniqueTransformationNameDocType"));
            return;
        }

        //Write info
        TransInfo.TransformationIsHierarchical = true;

        TransformationInfoProvider.SetTransformation(TransInfo);

        ShowChangesSaved();

        RaiseOnSaved();

        // Reload header if changes were saved
        ScriptHelper.RefreshTabHeader(Page, TransInfo.TransformationName);
    }

    #endregion
}
