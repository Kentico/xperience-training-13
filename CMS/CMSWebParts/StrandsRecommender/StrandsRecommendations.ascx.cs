using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.StrandsRecommender;
using CMS.StrandsRecommender.Web.UI;

public partial class CMSWebParts_StrandsRecommender_StrandsRecommendations : CMSAbstractWebPart
{
    #region "Constants"

    /// <summary>
    /// Separator used by Strands when passing multiple items
    /// </summary>
    public const string MULTIPLE_ITEMS_SEPARATOR = "_._";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets Recommendation identifier.
    /// </summary>
    public string RecommendationID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RecommendationTemplateIdentifier"), "");
        }
    }


    /// <summary>
    /// Gets or sets custom transformation code name.
    /// </summary>
    public string CustomTransformation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTransformation"), string.Empty);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var cookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();

        // Contains definition for position of error label
        CssRegistration.RegisterCssLink(Page, "~/CMSWebparts/StrandsRecommender/StrandsRecommendations.css");

        if (!StrandsSettings.IsStrandsEnabled(SiteContext.CurrentSiteName))
        {
            // If Strands is not currently available, stop processing webpart
            StopProcessing = true;
            HandleError(GetString("strands.notoken"));
        }
        else if (cookieLevelProvider.GetCurrentCookieLevel() < CookieLevel.Visitor)
        {
            // Do nothing, if user has decided to forbid cookie usage to under Visitor level, because Strands library uses Cookies under the hood
            StopProcessing = true;
            HandleError();
        }
        else
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        try
        {
            // Generate Strands javascript library code
            StrandsScriptRenderer renderer = new StrandsScriptRenderer(Page, StrandsSettings.GetApiID(SiteContext.CurrentSiteName));
            renderer.RenderCultureScript(DocumentContext.CurrentDocumentCulture.CultureCode);

            if (ECommerceContext.CurrentCurrency != null)
            {
                renderer.RenderCurrencyScript(ECommerceContext.CurrentCurrency.CurrencyCode);
            }

            renderer.RenderLibraryScript();

            var template = ParseRecommendationTemplate(RecommendationID);

            if (UseCustomTransformation())
            {
                RenderCustomTransformation(renderer, template);
            }

            // Sets template as tpl attribute in Strands div element. Strands portal uses it to determine, what recommendation template should be loaded.
            strandsRecs.Attributes.Add("tpl", template.ID);

            FillTemplateAttributes(template.Type);
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Strands Recommender", "STRANDS", ex);

            if (ex is StrandsException)
            {
                HandleError(((StrandsException)ex).UIMessage);
            }
            else
            {
                HandleError(ex.Message);        
            }
        }
    }


    /// <summary>
    /// Fills strands div element with attributes depending on given template type.
    /// </summary>
    /// <param name="strandsWebTemplateTypeEnum">Type of template</param>
    private void FillTemplateAttributes(StrandsWebTemplateTypeEnum strandsWebTemplateTypeEnum)
    {
        switch (strandsWebTemplateTypeEnum)
        {
            case StrandsWebTemplateTypeEnum.Home:
                // Selected widget doesn't need any item ID
                break;

            case StrandsWebTemplateTypeEnum.Product:
                // Current document ID will be sent to Strands

                if (CheckForProductPage()) // This template should not be displayed on documents not related with products
                {
                    strandsRecs.Attributes.Add("item", DocumentContext.CurrentDocument.NodeID.ToString());
                }
                    
                break;

            case StrandsWebTemplateTypeEnum.Cart:
                // Items IDs passed to Strands will be obtained from shopping cart

                var cartItems = StrandsProductsProvider.GetItemIDsFromCurrentShoppingCart().ToList();
                if (!cartItems.Any())
                {
                    HandleError(GetString("strands.recommendations.cart.noitem"));
                    break;
                }

                strandsRecs.Attributes.Add("item", GetItemsStringForMultipleItems(cartItems));
                break;

            case StrandsWebTemplateTypeEnum.Order:
                // Items IDs passed to Strands will be obtained from last order of current contact

                var orderItems = StrandsProductsProvider.GetItemsIDsFromRecentOrder().ToList();
                if (!orderItems.Any())
                {
                    HandleError(GetString("strands.recommendations.order.noorder"));
                    break;
                }

                strandsRecs.Attributes.Add("item", GetItemsStringForMultipleItems(orderItems));
                break;

            case StrandsWebTemplateTypeEnum.Category:
                // No items will be passed to Strands, only dfilter parameter will be set

                if (CheckForProductPage()) // This template should not be displayed on documents not related with products
                {
                    string category = StrandsCatalogPropertiesMapper.GetItemCategory(DocumentContext.CurrentDocument);
                    strandsRecs.Attributes.Add("dfilter", string.Format("category::{0}", category));
                }
                break;
        }
    }


    /// <summary>
    /// Checks, whether current document is product type. If not, hides webpart on live site and show warning in design.
    /// </summary>
    /// <returns>True if current document is product type, false otherwise</returns>
    private bool CheckForProductPage()
    {
        if (DocumentContext.CurrentDocument.IsProduct())
        {
            return true;
        }
        
        HandleError(GetString("strands.recommendations.product.wrongdocument"));
        return false;
    }


    /// <summary>
    /// Creates string representing multiple item IDs
    /// </summary>
    /// <param name="items">Collection of document IDs</param>
    /// <returns>String containing items from collection separated by Strands separator</returns>
    private static string GetItemsStringForMultipleItems(IEnumerable<string> items)
    {
        return string.Join(MULTIPLE_ITEMS_SEPARATOR, items);
    }


    /// <summary>
    /// Hides recommendation widget and displays error message when in design mode.
    /// </summary>
    /// <param name="message">Message to be displayed or nothing if message should not be displayed</param>
    private void HandleError(string message = null)
    {
        // If in design mode, hide recommendation
        if (PortalContext.IsDesignMode(PortalContext.ViewMode))
        {
            strandsRecs.Visible = false;
            if (!string.IsNullOrEmpty(message))
            {
                lblError.Text = message;
                lblError.Visible = true;
            }
        }
        else // Hide whole webpart
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Creates new instance of StrandsWebTemplateData from given value. Instance will have Type and ID properties defined.
    /// </summary>
    /// <param name="value">String to be parsed in form ("Int32;String")</param>
    /// <returns>Parsed StrandsWebTemplateData instance</returns>
    /// <exception cref="StrandsException">Input value is in bad format</exception>
    private StrandsWebTemplateData ParseRecommendationTemplate(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new StrandsException("[StrandsRecommendations.ParseRecommendationTemplate]: Given value was in a bad format (Int32;String expected)", GetString("strands.recommendations.templateidwrongformat"));
        }

        var recOptions = value.Split(';');
        if (recOptions.Length != 2)
        {
            throw new StrandsException("[StrandsRecommendations.ParseRecommendationTemplate]: Given value was in a bad format (Int32;String expected)", GetString("strands.recommendations.templateidwrongformat"));
        }

        StrandsWebTemplateTypeEnum type;
        if (!Enum.TryParse(recOptions[0], out type))
        {
            throw new StrandsException("[StrandsRecommendations.ParseRecommendationTemplate]: Given value was in a bad format (Int32;String expected)", GetString("strands.recommendations.templateidwrongformat"));
        }

        return new StrandsWebTemplateData
        {
            ID = recOptions[1],
            Type = type
        };
    }
    

    /// <summary>
    /// Returns true if custom transformation should be used to define visual style of strands widget.
    /// </summary>
    /// <returns>True if custom transformation should be used</returns>
    private bool UseCustomTransformation()
    {
        return !string.IsNullOrEmpty(CustomTransformation);
    }

    
    /// <summary>
    /// Renders script for custom transformation.
    /// </summary>
    /// <param name="renderer">Instance of Strands script renderer</param>
    /// <param name="template">Selected template</param>
    /// <exception cref="StrandsException">Transformation does not exist</exception>
    private void RenderCustomTransformation(StrandsScriptRenderer renderer, StrandsWebTemplateData template)
    {
        var transformation = TransformationInfoProvider.GetTransformation(CustomTransformation);

        if (transformation == null)
        {
            throw new StrandsException("[StrandsRecommendations.ParseRecommendationTemplate]: Cannot load the transformation specified in the web part.", ResHelper.GetString("strands.recommendations.transformations.errorloadingtransformation"));
        }
        if (transformation.TransformationType != TransformationTypeEnum.jQuery)
        {
            throw new StrandsException("[StrandsRecommendations.ParseRecommendationTemplate]: Transformation type has to be of type jQuery.", ResHelper.GetString("strands.recommendations.transformations.wrongtype"));
        }

        renderer.RenderCustomizedRendererScript(transformation, template.ID, strandsRecs.ClientID);
    }
    
    #endregion
}



