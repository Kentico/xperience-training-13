using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Controls_Filters_DiscountFilter : FormEngineUserControl
{
    private const string ITEM_SELECTALL = "-1";
    private const string ITEM_ACTIVE = "0";
    private const string ITEM_DISABLED = "1";
    private const string ITEM_FINISHED = "2";
    private const string ITEM_NOTSTARTED = "3";
    private const string ITEM_INCOMPLETE = "4";
    

    private DiscountTypeEnum mDiscountType = DiscountTypeEnum.Discount;
    

    /// <summary>
    /// Type of discount.
    /// </summary>
    public DiscountTypeEnum DiscountType
    {
        get
        {
            if (mDiscountType != DiscountTypeEnum.Discount)
            {
                return mDiscountType;
            }

            // Gets the value from the matching <filterparameter> element in the UniGrid's XML definition
            string parameterValue = GetValue("discounttype", String.Empty);
            if (!Enum.TryParse(parameterValue, true, out mDiscountType))
            {
                ShowError(string.Format(GetString("formcontrol.discountfilter.typeerror"), HTMLHelper.HTMLEncode(parameterValue)));
                
                return DiscountTypeEnum.Discount;
            }
            
            return mDiscountType;
        }
        set
        {
            SetValue("discounttype", value.ToStringRepresentation());
        }
    }


    public override object Value
    {
        get
        {
            return drpStatus.SelectedValue;
        }
        set
        {
            drpStatus.SelectedValue = ValidationHelper.GetString(value, ITEM_SELECTALL);
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!IsPostBack)
        {
            drpStatus.Items.Add(new ListItem(GetString("general.selectall"), ITEM_SELECTALL));
            drpStatus.Items.Add(new ListItem(GetString("com.discountstatus.active"), ITEM_ACTIVE));
            drpStatus.Items.Add(new ListItem(GetString("com.discountstatus.disabled"), ITEM_DISABLED));
            drpStatus.Items.Add(new ListItem(GetString("com.discountstatus.finished"), ITEM_FINISHED));
            drpStatus.Items.Add(new ListItem(GetString("com.discountstatus.notstarted"), ITEM_NOTSTARTED));

            // Catalog discount have no coupons, therefore can not have incomplete status
            if (DiscountType != DiscountTypeEnum.CatalogDiscount)
            {
                drpStatus.Items.Add(new ListItem(GetString("com.discountstatus.incomplete"), ITEM_INCOMPLETE));
            }
        }
    }


    public override string GetWhereCondition()
    {
        IWhereCondition query = new WhereCondition();

        switch (drpStatus.SelectedValue)
        {
            case ITEM_ACTIVE:
                query = GetActiveDiscounts();
                break;

            case ITEM_DISABLED:
                query = GetDisabledDiscounts();
                break;

            case ITEM_FINISHED:
                query = GetFinishedDiscounts();
                break;

            case ITEM_NOTSTARTED:
                query = GetNotStartedDiscounts();
                break;

            case ITEM_INCOMPLETE:
                query = GetIncompleteDiscounts();
                break;
        }

        return query.Expand(query.WhereCondition);
    }

    
    #region "Disabled discounts"

    private IWhereCondition GetDisabledDiscounts()
    {
        switch (DiscountType)
        {
            case DiscountTypeEnum.CatalogDiscount:
            case DiscountTypeEnum.OrderDiscount:
            case DiscountTypeEnum.ShippingDiscount:
                return DiscountInfo.Provider.Get().WhereFalse("DiscountEnabled");

            case DiscountTypeEnum.ProductCoupon:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO_PRODUCT_COUPON.TypeCondition.GetWhereCondition()).WhereFalse("MultiBuyDiscountEnabled");

            case DiscountTypeEnum.MultibuyDiscount:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO.TypeCondition.GetWhereCondition()).WhereFalse("MultiBuyDiscountEnabled");

            case DiscountTypeEnum.GiftCard:
                return GiftCardInfo.Provider.Get().WhereFalse("GiftCardEnabled");
        }

        return new WhereCondition();
    }

    #endregion


    #region "Active discounts"

    private IWhereCondition GetActiveDiscounts()
    {
        switch (DiscountType)
        {
            case DiscountTypeEnum.CatalogDiscount:
                return DiscountInfoProvider.GetRunningDiscounts(CurrentSite.SiteID, DateTime.Now, DiscountApplicationEnum.Products);

            case DiscountTypeEnum.OrderDiscount:
            case DiscountTypeEnum.ShippingDiscount:
                return DiscountInfoProvider.GetRunningDiscounts(CurrentSite.SiteID, DateTime.Now);

            case DiscountTypeEnum.ProductCoupon:
                return MultiBuyDiscountInfoProvider.GetRunningProductCouponDiscounts(CurrentSite.SiteID, DateTime.Now);

            case DiscountTypeEnum.MultibuyDiscount:
                return MultiBuyDiscountInfoProvider.GetRunningMultiBuyDiscounts(CurrentSite.SiteID, DateTime.Now);

            case DiscountTypeEnum.GiftCard:
                return GiftCardInfoProvider.GetRunningGiftCards(CurrentSite.SiteID, DateTime.Now);
        }

        return new WhereCondition();
    }
    
    #endregion


    #region "Incomplete discounts"
    
    private IWhereCondition GetIncompleteDiscounts()
    {
        switch (DiscountType)
        {
            // Catalog discounts can't be incomplete
            case DiscountTypeEnum.OrderDiscount:
            case DiscountTypeEnum.ShippingDiscount:
                return DiscountInfo.Provider.Get().Where(IncompleteDiscountsCondition());

            case DiscountTypeEnum.ProductCoupon:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO_PRODUCT_COUPON.TypeCondition.GetWhereCondition()).Where(IncompleteMultiBuyDiscountsCondition());

            case DiscountTypeEnum.MultibuyDiscount:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO.TypeCondition.GetWhereCondition()).Where(IncompleteMultiBuyDiscountsCondition());

            case DiscountTypeEnum.GiftCard:
                return GiftCardInfo.Provider.Get().Where(IncompleteGiftCardsCondition());
        }

        return new WhereCondition();
    }


    private IWhereCondition IncompleteDiscountsCondition()
    {
        return new WhereCondition()
            .WhereTrue("DiscountEnabled")
            .WhereTrue("DiscountUsesCoupons")
            .WhereNotIn("DiscountID", new IDQuery(CouponCodeInfo.OBJECT_TYPE, "CouponCodeDiscountID"));
    }


    private IWhereCondition IncompleteMultiBuyDiscountsCondition()
    {
        return new WhereCondition()
            .WhereTrue("MultiBuyDiscountEnabled")
            .WhereTrue("MultiBuyDiscountUsesCoupons")
            .WhereNotIn("MultiBuyDiscountID", new IDQuery(MultiBuyCouponCodeInfo.OBJECT_TYPE, "MultiBuyCouponCodeMultiBuyDiscountID"));
    }


    private IWhereCondition IncompleteGiftCardsCondition()
    {
        return new WhereCondition()
            .WhereTrue("GiftCardEnabled")
            .WhereNotIn("GiftCardID", new IDQuery(GiftCardCouponCodeInfo.OBJECT_TYPE, "GiftCardCouponCodeGiftCardID"));
    }

    #endregion


    #region "Not started discounts"

    private IWhereCondition GetNotStartedDiscounts()
    {
        switch (DiscountType)
        {
            case DiscountTypeEnum.CatalogDiscount:
                return DiscountInfo.Provider.Get()
                                           .WhereTrue("DiscountEnabled")
                                           .WhereNotNull("DiscountValidFrom").WhereGreaterThan("DiscountValidFrom", DateTime.Now);

            case DiscountTypeEnum.OrderDiscount:
            case DiscountTypeEnum.ShippingDiscount:
                return DiscountInfo.Provider.Get()
                                           .WhereTrue("DiscountEnabled")
                                           .WhereNot(IncompleteDiscountsCondition())
                                           .WhereNotNull("DiscountValidFrom").WhereGreaterThan("DiscountValidFrom", DateTime.Now);

            case DiscountTypeEnum.ProductCoupon:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO_PRODUCT_COUPON.TypeCondition.GetWhereCondition()).Where(NotStartedMultiBuyDiscountsCondition());

            case DiscountTypeEnum.MultibuyDiscount:
                return MultiBuyDiscountInfo.Provider.Get().Where(MultiBuyDiscountInfo.TYPEINFO.TypeCondition.GetWhereCondition()).Where(NotStartedMultiBuyDiscountsCondition());

            case DiscountTypeEnum.GiftCard:
                return GiftCardInfo.Provider.Get()
                                            .WhereTrue("GiftCardEnabled")
                                            .WhereNot(IncompleteGiftCardsCondition())
                                            .WhereNotNull("GiftCardValidFrom").WhereGreaterThan("GiftCardValidFrom", DateTime.Now);
        }

        return new WhereCondition();
    }


    private IWhereCondition NotStartedMultiBuyDiscountsCondition()
    {
        return new WhereCondition()
            .WhereTrue("MultiBuyDiscountEnabled")
            .WhereNot(IncompleteMultiBuyDiscountsCondition())
            .WhereNotNull("MultiBuyDiscountValidFrom").WhereGreaterThan("MultiBuyDiscountValidFrom", DateTime.Now);
    }

    #endregion


    #region "Finished discounts"

    private IWhereCondition GetFinishedDiscounts()
    {
        switch (DiscountType)
        {
            case DiscountTypeEnum.CatalogDiscount:
                return new WhereCondition()
                    // Discount is enabled
                    .WhereTrue("DiscountEnabled")
                    // Discount had been valid in past
                    .WhereNotNull("DiscountValidTo").WhereLessThan("DiscountValidTo", DateTime.Now);

            case DiscountTypeEnum.OrderDiscount:
            case DiscountTypeEnum.ShippingDiscount:
                return new WhereCondition()
                    // Discount is enabled
                    .WhereTrue("DiscountEnabled")
                    // Discount is either running or finished
                    .Where(new WhereCondition().WhereLessOrEquals("DiscountValidFrom", DateTime.Now).Or().WhereNull("DiscountValidFrom"))
                    // Discount is not incomplete
                    .Where(new WhereCondition().WhereFalse("DiscountUsesCoupons").Or().WhereIn("DiscountID", new IDQuery(CouponCodeInfo.OBJECT_TYPE, "CouponCodeDiscountID")))
                    .Where(new WhereCondition()
                        // Discount is finished, and we don't care about coupons
                        .WhereLessThan("DiscountValidTo", DateTime.Now)
                        .Or()
                        // Discount is running and all it's coupons are exceeded, if it uses any
                        .Where(new WhereCondition()
                            .WhereTrue("DiscountUsesCoupons")
                            .WhereNotIn("DiscountID", new IDQuery(CouponCodeInfo.OBJECT_TYPE, "CouponCodeDiscountID")
                                .WhereNull("CouponCodeUseLimit")
                                .Or()
                                .Where("CouponCodeUseCount < CouponCodeUseLimit"))));

            case DiscountTypeEnum.MultibuyDiscount:
            case DiscountTypeEnum.ProductCoupon:
                return new WhereCondition()
                    // Multibuy discount is enabled
                    .WhereTrue("MultiBuyDiscountEnabled")
                    // Multibuy discount is either running or finished
                    .Where(new WhereCondition().WhereLessOrEquals("MultiBuyDiscountValidFrom", DateTime.Now).Or().WhereNull("MultiBuyDiscountValidFrom"))
                    // Multibuy discount is not incomplete
                    .Where(new WhereCondition()
                        .WhereFalse("MultiBuyDiscountUsesCoupons")
                        .Or()
                        .WhereIn("MultiBuyDiscountID", new IDQuery(MultiBuyCouponCodeInfo.OBJECT_TYPE, "MultiBuyCouponCodeMultiBuyDiscountID")))
                    .Where(new WhereCondition()
                        // Multibuy discount is finished, and we don't care about coupons
                        .WhereLessThan("MultiBuyDiscountValidTo", DateTime.Now)
                        .Or()
                        // Multibuy discount is running and all it's coupons are exceeded, if it uses any
                        .Where(new WhereCondition()
                            .WhereTrue("MultiBuyDiscountUsesCoupons")
                            .WhereNotIn("MultiBuyDiscountID", new IDQuery(MultiBuyCouponCodeInfo.OBJECT_TYPE, "MultiBuyCouponCodeMultiBuyDiscountID")
                                .WhereNull("MultiBuyCouponCodeUseLimit")
                                .Or()
                                .Where("MultiBuyCouponCodeUseCount < MultiBuyCouponCodeUseLimit"))));

            case DiscountTypeEnum.GiftCard:
                return new WhereCondition()
                    // Gift card is enabled
                    .WhereTrue("GiftCardEnabled")
                    // Gift card is either running or finished
                    .Where(new WhereCondition().WhereLessOrEquals("GiftCardValidFrom", DateTime.Now).Or().WhereNull("GiftCardValidFrom"))
                    // Gift card is not incomplete
                    .WhereIn("GiftCardID", new IDQuery(GiftCardCouponCodeInfo.OBJECT_TYPE, "GiftCardCouponCodeGiftCardID"))
                    .Where(new WhereCondition()
                        // Gift card is finished, and we don't care about coupons
                        .WhereLessThan("GiftCardValidTo", DateTime.Now)
                        .Or()
                        // Gift card is running and all it's coupons are exceeded
                        .WhereNotIn("GiftCardID", new IDQuery(GiftCardCouponCodeInfo.OBJECT_TYPE, "GiftCardCouponCodeGiftCardID").WhereGreaterThan("GiftCardCouponCodeRemainingValue", 0)));
        }

        return new WhereCondition();
    }

    #endregion
}