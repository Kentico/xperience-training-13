using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WebAnalytics.Web.UI;


[Action(0, "conversion.conversion.new", "Edit.aspx")]
[Security(Resource = "CMS.WebAnalytics", UIElements = "Conversions;Conversions.List")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_List : CMSConversionPage
{
    private const string SMART_TIP_IDENTIFIER = "conversions|listing";
    private const string CONVERSIONS_LOGGING_LINK = "conversions_logging";

    protected void Page_Load(object sender, EventArgs e)
    {
        var documentationLink = DocumentationHelper.GetDocumentationTopicUrl(CONVERSIONS_LOGGING_LINK);

        tipConversionsListing.CollapsedStateIdentifier = SMART_TIP_IDENTIFIER;                
        tipConversionsListing.Content = string.Format(GetString("conversions.listingsmarttip"), documentationLink);
        tipConversionsListing.ExpandedHeader = GetString("conversions.listingsmarttip.header");
    }
}