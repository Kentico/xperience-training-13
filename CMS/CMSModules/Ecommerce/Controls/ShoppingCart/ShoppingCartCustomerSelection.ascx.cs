using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[CheckLicence(FeatureEnum.Ecommerce)]
public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCustomerSelection : ShoppingCartStep
{
    #region "Variables and properties"

    private bool? mIsExistingCustomer;
    private bool? mHasModifyCustomerPermission;

    private bool ExistingCustomer
    {
        get
        {
            if (mIsExistingCustomer == null)
            {
                mIsExistingCustomer = (btnGroup.SelectedActionName == "existing_customer");
            }

            return mIsExistingCustomer.Value;
        }
    }


    private bool HasModifyCustomerPermission
    {
        get
        {
            if (mHasModifyCustomerPermission == null)
            {
                mHasModifyCustomerPermission = ECommerceContext.IsUserAuthorizedToModifyCustomer();
            }

            return mHasModifyCustomerPermission.Value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        var btnExistingCustomer = new CMSButtonGroupAction
              {
                  Text = GetString("com.ui.existingcustomer"),
                  Name = "existing_customer"
              };

        var btnNewCustomer = new CMSButtonGroupAction
        {
            Text = GetString("com.ui.newcustomer"),
            Name = "new_customer",
            Enabled = HasModifyCustomerPermission
        };

        btnGroup.Actions.Add(btnExistingCustomer);
        btnGroup.Actions.Add(btnNewCustomer);
        btnGroup.AutomaticButtonSelection = true;

        // Hide default submit button
        customerForm.SubmitButton.Visible = false;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        plcMess.BasicStyles = true;

        // Mark previously selected customer
        if ((!ShoppingCartControl.IsCurrentStepPostBack) && (ShoppingCart.ShoppingCartCustomerID > 0))
        {
            customerSelector.CustomerID = ShoppingCart.ShoppingCartCustomerID;
        }

        ShoppingCartControl.ButtonBack.Visible = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        customerSelector.Visible = ExistingCustomer;
        customerForm.Visible = !ExistingCustomer;
        lblSelectError.Visible = !string.IsNullOrEmpty(lblSelectError.Text);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Check if form is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (ExistingCustomer)
        {
            // Check if customer is selected
            if (customerSelector.CustomerID > 0)
            {
                return true;
            }

            lblSelectError.Text = GetString("shoppingcartselectcustomer.errorselect");
            return false;
        }

        if (!ExistingCustomer)
        {
            var reg = ValidationHelper.GetBoolean(customerForm.GetFieldValue("Register"), false);
            if (reg)
            {
                var email = customerForm.GetFieldValue("CustomerEmail").ToString();

                // Check whether email is unique if it is required
                if (!UserInfoProvider.IsEmailUnique(email, CurrentSite.SiteName, 0))
                {
                    ShowError(GetString("UserInfo.EmailAlreadyExist"));
                    return false;
                }
            }
        }

        return ExistingCustomer || customerForm.ValidateData();
    }


    /// <summary>
    /// Process form data.
    /// </summary>
    public override bool ProcessStep()
    {
        int customerId = 0;

        if (ExistingCustomer)
        {
            // Select an existing customer
            customerId = customerSelector.CustomerID;
        }

        if (!ExistingCustomer)
        {
            // Check permissions
            if (!HasModifyCustomerPermission)
            {
                RedirectToAccessDenied(ModuleName.ECOMMERCE, "ModifyCustomers OR EcommerceModify");
                return false;
            }

            // Create a new customer
            customerForm.SaveData(null, false);

            var customer = (CustomerInfo)EditedObject;
            customerId = customer.CustomerID;

            var reg = ValidationHelper.GetBoolean(customerForm.GetFieldValue("Register"), false);
            if (reg)
            {
                CustomerInfoProvider.RegisterAndNotify(customer, "Ecommerce.AutomaticRegistration");
            }
        }

        // Assign customer and user to the shopping cart
        ShoppingCart.SetShoppingCartUser(customerId);

        return customerId > 0;
    }

    #endregion
}