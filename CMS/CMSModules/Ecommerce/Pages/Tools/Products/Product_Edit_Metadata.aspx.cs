using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Products.Metadata")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Metadata : CMSProductsPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Node != null)
        {
            // Check read permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(menuElem.Node.GetDocumentName())));
            }
        }

        // Enable split mode
        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        MetaDataControlExtender extender = new MetaDataControlExtender();
        extender.UIModuleName = ModuleName.ECOMMERCE;
        extender.UIMetadataElementName = "Products.MetaData";
        extender.Init(editForm);       
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If modify is allowed
        pnlContent.Enabled = DocumentManager.AllowSave;
    }

    #endregion
}
