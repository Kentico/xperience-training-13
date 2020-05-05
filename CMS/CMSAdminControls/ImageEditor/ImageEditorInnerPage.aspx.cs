using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_ImageEditor_ImageEditorInnerPage : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("imageeditor.badhashtitle", "imageeditor.badhashtext"));
        }
        else
        {
            ScriptHelper.RegisterJQueryCrop(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/ImageEditor/ImageEditorInnerPage.js");
            CssRegistration.RegisterBootstrap(Page);

            string imgUrl = QueryHelper.GetString("imgurl", null);
            if (String.IsNullOrEmpty(imgUrl))
            {
                string query = RequestContext.CurrentQueryString;

                query = URLHelper.RemoveParameterFromUrl(query, "hash");

                var settings = new HashSettings(HashValidationSalts.GET_IMAGE_VERSION);

                query = URLHelper.AddParameterToUrl(query, "hash", ValidationHelper.GetHashString(query, settings));

                imgContent.ImageUrl = UrlResolver.ResolveUrl("~/CMSPages/GetImageVersion.aspx" + query);

                int imgwidth = QueryHelper.GetInteger("imgwidth", 0);
                int imgheight = QueryHelper.GetInteger("imgheight", 0);
                if ((imgwidth > 0) && (imgheight > 0))
                {
                    imgContent.Width = imgwidth;
                    imgContent.Height = imgheight;
                }
            }
            else
            {
                imgContent.ImageUrl = imgUrl;
            }
        }
    }
}
