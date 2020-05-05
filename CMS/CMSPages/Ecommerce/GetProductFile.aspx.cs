using System;

using CMS.AspNet.Platform.Routing;
using CMS.Base;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Routing.Web;
using CMS.UIControls;

public partial class CMSPages_Ecommerce_GetProductFile : AbstractCMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set default error message
        var errorMessageResString = "getproductfile.existerror";

        // Get order item SKU file
        var token = QueryHelper.GetGuid("token", Guid.Empty);
        var orderedFile = OrderItemSKUFileInfo.Provider.Get(token);
        if (orderedFile != null)
        {
            // Get parent order item
            var orderItem = OrderItemInfo.Provider.Get(orderedFile.OrderItemID);
            if (orderItem != null)
            {
                // If download is not expired
                if ((orderItem.OrderItemValidTo.CompareTo(DateTimeHelper.ZERO_TIME) == 0) || (orderItem.OrderItemValidTo.CompareTo(DateTime.Now) > 0))
                {
                    // Get SKU file
                    var skuFile = SKUFileInfo.Provider.Get(orderedFile.FileID);

                    if (skuFile != null)
                    {
                        // Decide how to process the file based on file type
                        switch (skuFile.FileType.ToLowerCSafe())
                        {
                            case "metafile":
                                // Set parameters to current context
                                Context.Items["fileguid"] = skuFile.FileMetaFileGUID;
                                Context.Items["disposition"] = "attachment";

                                // Handle request using metafile handler
                                Response.Clear();
                                var handler = new ActionResultRouteHandler<GetMetafileService>()
                                    .GetHttpHandler(Request.RequestContext);

                                handler.ProcessRequest(Context);
                                Response.End();

                                return;
                        }
                    }
                }
                else
                {
                    // Set error message
                    errorMessageResString = "getproductfile.expirederror";
                }
            }
        }

        // Perform server side redirect to error page
        Response.Clear();
        Server.Transfer(AdministrationUrlHelper.GetErrorPageUrl("getproductfile.error", errorMessageResString));
        Response.End();
    }
}