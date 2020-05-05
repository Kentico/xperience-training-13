using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_MVTVariant_Edit : CMSAdminEditControl
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

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Visible)
        {
            EditForm.OnAfterSave += EditForm_OnAfterSave;
        }
    }


    /// <summary>
    /// Handles the OnAfterSave event of the EditForm control.
    /// </summary>
    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        if (UIFormControl.EditedObject != null)
        {
            // Log widget variant synchronization
            MVTVariantInfo variantInfo = (MVTVariantInfo)UIFormControl.EditedObject;

            // Clear cache
            CacheHelper.TouchKey("om.mvtvariant|bytemplateid|" + variantInfo.MVTVariantPageTemplateID);

            if (variantInfo.MVTVariantDocumentID > 0)
            {
                // Log synchronization
                TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNode node = tree.SelectSingleDocument(variantInfo.MVTVariantDocumentID);
                DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
            }
        }
    }

    #endregion
}