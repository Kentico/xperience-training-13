using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCart : ShoppingCart
{
    /// <summary>
    /// Back button.
    /// </summary>
    public override CMSButton ButtonBack
    {
        get
        {
            return btnBack;
        }
        set
        {
            btnBack = value;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public override CMSButton ButtonNext
    {
        get
        {
            return btnNext;
        }
        set
        {
            btnNext = value;
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var currentSite = SiteContext.CurrentSite;

        // If shopping cart is created -> create empty one
        if ((ShoppingCartInfoObj == null) && (currentSite != null))
        {
            var currentUser = MembershipContext.AuthenticatedUser;

            ShoppingCartInfoObj = ShoppingCartFactory.CreateCart(currentSite.SiteID, currentUser);
        }

        // Display / hide checkout process images
        plcCheckoutProcess.Visible = false;

        // Load current step data
        LoadCurrentStep();

        if (CurrentStepIndex == 0)
        {
            ShoppingCartInfoObj.PrivateDataCleared = false;
            btnBack.Enabled = false;
        }

        // If shopping cart information exist
        if (ShoppingCartInfoObj != null)
        {
            // Get order information
            var order = OrderInfo.Provider.Get(ShoppingCartInfoObj.OrderId);

            // If order is paid
            if ((order != null) && (order.OrderIsPaid))
            {
                // Disable specific controls
                btnNext.Enabled = false;
                CurrentStepControl.Enabled = false;
            }
        }
    }


    /// <summary>
    /// On page pre-render event.
    /// </summary>
    protected void Page_Prerender(object sender, EventArgs e)
    {
        // Back and Next button disabling script (to prevent multiple click)
        ButtonNext.OnClientClick = "if ((typeof(Page_ClientValidate) == 'function') && Page_ClientValidate('" + ButtonNext.ValidationGroup + "')) { this.disabled = true; } " + Page.ClientScript.GetPostBackEventReference(ButtonNext, null) + "; return false;";
        ButtonBack.OnClientClick = "if ((typeof(Page_ClientValidate) == 'function') && Page_ClientValidate('" + ButtonBack.ValidationGroup + "')) { this.disabled = true; }" + Page.ClientScript.GetPostBackEventReference(ButtonBack, null) + "; return false;";

        if ((CheckoutProcessSteps != null) && (CurrentStepControl != null))
        {
            lblStepTitle.Text = HTMLHelper.HTMLEncode(String.Format(GetString("Order_New.CurrentStep"), CurrentStepIndex + 1, CheckoutProcessSteps.Count));
            headStepTitle.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(CheckoutProcessSteps[CurrentStepIndex].Caption));
        }
        else
        {
            ButtonBack.Visible = false;
            ButtonNext.Visible = false;

            // Display error message, when no steps found
            if ((CheckoutProcessSteps == null) || (CheckoutProcessSteps.Count == 0))
            {
                lblError.Text = GetString("com.checkoutprocess.nosteps");
                lblError.Visible = true;
            }
        }
    }


    /// <summary>
    /// Back button clicked.
    /// </summary>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        // Load first checkout process step if private data was cleared
        if (ShoppingCartInfoObj.PrivateDataCleared && (CurrentStepIndex > 0))
        {
            ShoppingCartInfoObj.PrivateDataCleared = false;
            LoadStep(0);

            lblError.Visible = true;
            lblError.Text = GetString("com.shoppingcart.sessiontimedout");
            return;
        }

        CurrentStepControl.ButtonBackClickAction();
    }


    /// <summary>
    /// Next button clicked.
    /// </summary>
    protected void btnNext_Click(object sender, EventArgs e)
    {
        // Check private data
        if (ShoppingCartInfoObj.PrivateDataCleared && (CurrentStepIndex > 0))
        {
            // Go to the first step
            ShoppingCartInfoObj.PrivateDataCleared = false;
            LoadStep(0);

            // Display error
            lblError.Visible = true;
            lblError.Text = GetString("com.shoppingcart.sessiontimedout");
            return;
        }

        CurrentStepControl.ButtonNextClickAction();
    }


    /// <summary>
    /// Loads current step control.
    /// </summary>    
    public override void LoadCurrentStep()
    {
        if ((CurrentStepIndex >= 0) && (CurrentStepIndex < CheckoutProcessSteps.Count))
        {
            // Shopping cart container
            ShoppingCartContainer = pnlShoppingCart;

            // Default button settings
            ButtonBack.Enabled = true;
            ButtonNext.Enabled = true;
            ButtonBack.Visible = true;
            ButtonNext.Visible = true;
            ButtonBack.Text = GetString("general.back");
            ButtonNext.Text = GetString("general.next");
            ButtonBack.ButtonStyle = ButtonStyle.Primary;
            ButtonNext.ButtonStyle = ButtonStyle.Primary;

            if (CurrentStepControl != null)
            {
                // Display current control      
                pnlCartStepInner.Controls.Clear();
                pnlCartStepInner.Controls.Add(CurrentStepControl);
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = GetString("ShoppingCart.ErrorLoadingStep");
            }
        }
    }
}