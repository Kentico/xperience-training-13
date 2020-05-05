using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[Breadcrumb(1, "com.discount.generatecoupons")]
public partial class CMSModules_Ecommerce_Pages_Tools_Discount_Discount_Codes_Generator : CMSEcommercePage
{
    #region "Variables and constants"

    private int mDiscountId;
    private string mDiscountObjectType = string.Empty;
    private IDiscountInfo mDiscount;
    private int count;
    private string prefix = "";
    private int numberOfUses;
    private string redirectUrl;
    private string mElementName;
    private const string pattern = "*****";

    #endregion


    #region "Properties"

    /// <summary>
    /// Discount id from query.
    /// </summary>
    private int DiscountID
    {
        get
        {
            if (mDiscountId == 0)
            {
                mDiscountId = QueryHelper.GetInteger("discountId", 0);
            }
            return mDiscountId;
        }
    }


    /// <summary>
    /// Discount object type from query.
    /// </summary>
    private string DiscountObjectType
    {
        get
        {
            if (string.IsNullOrEmpty(mDiscountObjectType))
            {
                mDiscountObjectType = QueryHelper.GetString("discountObjectType", string.Empty);
            }

            return mDiscountObjectType;
        }
    }


    /// <summary>
    /// Gets the discount info from DB based on Discount type and Discount Id.
    /// </summary>
    private IDiscountInfo Discount
    {
        get
        {
            return mDiscount ?? (mDiscount = (IDiscountInfo)ProviderHelper.GetInfoById(DiscountObjectType, DiscountID, true));
        }
    }


    /// <summary>
    /// Gets the name of the page element.
    /// </summary>    
    private string ElementName
    {
        get
        {
            if (string.IsNullOrEmpty(mElementName))
            {
                switch (Discount.DiscountType)
                {
                    case DiscountTypeEnum.MultibuyDiscount:
                        mElementName = "MultiBuyCouponsCodes";
                        break;

                    case DiscountTypeEnum.ProductCoupon:
                        mElementName = "ProductCouponCodes";
                        break;

                    case DiscountTypeEnum.OrderDiscount:
                        mElementName = "OrderCouponCodes";
                        break;

                    case DiscountTypeEnum.ShippingDiscount:
                        mElementName = "ShippingCouponCodes";
                        break;

                    case DiscountTypeEnum.GiftCard:
                        mElementName = "GiftCardCouponCodes";
                        break;
                }
            }

            return mElementName;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Parent discount does not exist or it is a catalog discount where coupons are not allowed
        if (Discount == null || Discount.DiscountType == DiscountTypeEnum.CatalogDiscount)
        {
            EditedObjectParent = null;
            return;
        }

        // Check if object from current site is edited
        CheckEditedObjectSiteID(Discount.DiscountSiteID);

        // Check UI permissions
        CheckUIPermissions();

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("com.couponcode.generate"),
            CommandName = "generate"
        });

        redirectUrl = GetRedirectUrl();
        SetBreadcrumb(0, GetString("com.discount.coupons"), redirectUrl, null, null);

        // Hide count limitation for gift cards
        plcTimesToUse.Visible = (Discount.DiscountType != DiscountTypeEnum.GiftCard);

        if (!RequestHelper.IsPostBack())
        {
            // Show error message
            if (QueryHelper.Contains("error"))
            {
                ShowError(HTMLHelper.HTMLEncode(QueryHelper.GetString("error", string.Empty)));
            }
        }

        // Setup and configure asynchronous control
        SetupAsyncControl();
    }

    #endregion


    #region "Event handlers"

    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "generate")
        {
            if (!DiscountInfoProvider.IsUserAuthorizedToModifyDiscount(SiteContext.CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyDiscounts");
            }

            // Collect data from form
            count = ValidationHelper.GetInteger(txtNumberOfCodes.Text, 0);
            numberOfUses = ValidationHelper.GetInteger(txtTimesToUse.Text, int.MinValue);
            prefix = txtPrefix.Text.Trim();

            // Validate inputs
            if (!ValidateInputs())
            {
                return;
            }

            // Set numberOfUses to 0 for empty
            if (string.IsNullOrEmpty(txtTimesToUse.Text))
            {
                numberOfUses = 0;
            }

            // Run action in asynchronous control
            EnsureAsyncLog();
            RunAsync(GenerateCodes);
        }
    }


    /// <summary>
    /// Checks page UI permissions based on parent discount type.
    /// </summary>
    private void CheckUIPermissions()
    {
        // Check UI personalization
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, ElementName);
    }

    #endregion


    #region "Methods"

    private string GetRedirectUrl()
    {
        if (Discount != null && !string.IsNullOrEmpty(ElementName))
        {
            var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, ElementName);
            url = URLHelper.AddParameterToUrl(url, "parentobjectid", Discount.DiscountID.ToString());
            url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");
            return url;
        }

        return RequestContext.CurrentURL;
    }


    private IEnumerable<string> GetExistingCodes()
    {
        // Prepare query for codes cache
        var existingQuery = ECommerceHelper.GetAllCouponCodesQuery(SiteContext.CurrentSiteID);

        // Restrict cache if prefix specified
        if (!string.IsNullOrEmpty(prefix))
        {
            existingQuery.WhereStartsWith("CouponCodeCode", prefix);
        }

        // Create cache of this site coupon codes
        return existingQuery.GetListResult<string>();
    }


    private void GenerateCodes(object parameter)
    {
        try
        {
            // Construct cache for code uniqueness check
            var existingCodes = GetExistingCodes();
            BaseInfo coupon = null;

            using (var context = new CMSActionContext())
            {
                // Do not touch parent for all codes
                context.TouchParent = false;
                context.LogEvents = false;

                var logMessage = GetString("com.couponcode.generated");

                // Create generator
                var generator = new RandomCodeGenerator(new CodeUniquenessChecker(existingCodes), pattern, prefix);

                for (var i = 0; i < count; i++)
                {
                    // Get new code
                    var code = generator.GenerateCode();
                    var couponConfig = GetCouponConfig(code);

                    coupon = Discount.CreateCoupon(couponConfig);

                    // Log that coupon was created
                    AddLog(string.Format(logMessage, HTMLHelper.HTMLEncode(code)));
                }
            }

            // Touch parent (one for all)
            coupon?.Generalized.TouchParent();

            // Log information that coupons were generated
            var logData = new EventLogData(EventTypeEnum.Information, "Discounts", "GENERATECOUPONCODES")
            {
                EventDescription = $"{count} coupon codes for discount '{Discount.DiscountDisplayName}' successfully generated.",
                UserID = CurrentUser.UserID,
                UserName = CurrentUser.UserName,
                SiteID = SiteContext.CurrentSiteID
            };
            
            Service.Resolve<IEventLogService>().LogEvent(logData);
        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.couponcode.generateerror");
            Service.Resolve<IEventLogService>().LogException("Discounts", "GENERATECOUPONCODES", ex);
        }
    }


    /// <summary>
    /// Adds parameter to current URL and Redirects to it.
    /// </summary>
    /// <param name="parameter">Parameter to be added.</param>
    /// <param name="value">Value of parameter to be added.</param>
    private void RedirectTo(string parameter, string value)
    {
        string urlToRedirect = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, parameter, value);
        URLHelper.Redirect(urlToRedirect);
    }


    private CouponGeneratorConfig GetCouponConfig(string code)
    {
        return Discount.DiscountType == DiscountTypeEnum.GiftCard 
            ? new CouponGeneratorConfig(code) 
            : new CouponGeneratorConfig(code, numberOfUses);
    }


    private bool ValidateInputs()
    {
        if (count < 1)
        {
            ShowError(GetString("com.couponcode.invalidcount"));
            return false;
        }

        if (Discount.DiscountType != DiscountTypeEnum.GiftCard && !string.IsNullOrEmpty(txtTimesToUse.Text) && ((numberOfUses <= 0) || (numberOfUses > 999999)))
        {
            ShowError(GetString("com.couponcode.invalidnumberOfUses"));
            return false;
        }

        if (!string.IsNullOrEmpty(prefix) && !ValidationHelper.IsCodeName(prefix))
        {
            ShowError(GetString("com.couponcode.invalidprefix"));
            return false;
        }

        return true;
    }

    #endregion


    #region "Handling asynchronous thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        RedirectTo("error", GetString("com.couponcode.generationterminated"));
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            RedirectTo("error", CurrentError);
        }

        URLHelper.Redirect(UrlResolver.ResolveUrl(redirectUrl));
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlGeneral.Visible = false;
        CurrentError = string.Empty;
    }


    /// <summary>
    /// Runs asynchronous thread
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Prepare asynchronous control
    /// </summary>
    private void SetupAsyncControl()
    {
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.MaxLogLines = 1000;

        // Asynchronous content configuration
        ctlAsyncLog.TitleText = GetString("com.couponcode.generating");
        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlGeneral.Visible = true;
            pnlLog.Visible = false;
        }
    }

    #endregion
}