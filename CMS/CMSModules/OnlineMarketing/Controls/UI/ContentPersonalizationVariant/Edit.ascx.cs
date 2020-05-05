using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_ContentPersonalizationVariant_Edit : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
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
            EditForm.StopProcessing = value;
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
            EditForm.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Visible)
        {
            if (EditForm.FieldControls != null)
            {
                EditForm.FieldControls["variantdisplaycondition"].SetValue("ResolverName", "VariantResolver");
            }
            EditForm.OnAfterSave += EditForm_OnAfterSave;
        }
    }


    /// <summary>
    /// Handles the OnAfterSave event of the EditForm control.
    /// </summary>
    protected void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        if (EditForm.EditedObject != null)
        {
            // Log widget variant synchronization
            ContentPersonalizationVariantInfo variantInfo = (ContentPersonalizationVariantInfo)UIFormControl.EditedObject;

            // Clear cache
            CacheHelper.TouchKey("om.personalizationvariant|bytemplateid|" + variantInfo.VariantPageTemplateID);

            if (variantInfo.VariantDocumentID > 0)
            {
                // Log synchronization
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNode node = tree.SelectSingleDocument(variantInfo.VariantDocumentID);
                DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Validates the form data. Checks the code name format and if the code name is unique.
    /// </summary>
    public bool ValidateData()
    {
        // Check the required fields for emptiness
        bool isValid = EditForm.ValidateData();

        if (isValid)
        {
            if ((EditForm.EditedObject == null) || (EditForm.ParentObject == null))
            {
                EditForm.ShowError(GetString("general.saveerror"));
                return false;
            }

            var variantInfo = (ContentPersonalizationVariantInfo)EditForm.EditedObject;
            variantInfo.VariantPageTemplateID = EditForm.ParentObject.Generalized.ObjectID;

            if (QueryHelper.GetString("varianttype", null) == "widget")
            {
                variantInfo.VariantDocumentID = DocumentContext.CurrentDocument.DocumentID;
            }

            // Ensures code name is automatically created if code name field left blank
            variantInfo.Generalized.EnsureCodeName();

            // Check if the code name already exists
            if (!variantInfo.CheckUniqueCodeName())
            {
                isValid = false;
                string niceObjectType = TypeHelper.GetNiceObjectTypeName(EditForm.ObjectType);
                EditForm.ShowError(String.Format(GetString("general.codenamenotunique"), niceObjectType, HTMLHelper.HTMLEncode(EditForm.EditedObject.GetStringValue("VariantName", string.Empty))));
            }
        }

        return isValid;
    }

    #endregion
}