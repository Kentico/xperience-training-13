using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_DragOperation : CMSContentPage
{
    #region "Properties"

    /// <summary>
    /// Indicates if deleting products/products section in ecommerce context.
    /// </summary>
    private bool IsProductsMode
    {
        get
        {
            return QueryHelper.GetString("mode", "").ToLowerCSafe() == "productssection";
        }
    }


    /// <summary>
    /// Indicates if page shown in products UI
    /// </summary>
    protected override bool IsProductsUI
    {
        get
        {
            return IsProductsMode;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect for non-existing document
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            string action = QueryHelper.GetString("action", "");
            string titleText = null;

            switch (action.ToLowerCSafe())
            {
                case "movenode":
                case "movenodeposition":
                case "movenodefirst":
                    {
                        // Setup page title text and image
                        titleText = GetString("dialogs.header.title.movedoc");
                    }
                    break;

                case "copynode":
                case "copynodeposition":
                case "copynodefirst":
                    {
                        // Setup page title text and image
                        titleText = GetString("dialogs.header.title.copydoc");
                    }
                    break;

                case "linknode":
                case "linknodeposition":
                case "linknodefirst":
                    {
                        // Setup page title text and image
                        titleText = GetString("dialogs.header.title.linkdoc");
                    }
                    break;
            }

            PageTitle.TitleText = titleText;
            EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);
            ((Panel)CurrentMaster.PanelBody.FindControl("pnlContent")).CssClass = string.Empty;
        }
    }

    #endregion
}