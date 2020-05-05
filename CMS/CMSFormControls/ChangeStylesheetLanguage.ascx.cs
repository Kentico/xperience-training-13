using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Synchronization;


public partial class CMSFormControls_ChangeStylesheetLanguage : FormEngineUserControl
{
    #region "Variables"

    private string mValue = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value of the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return ddlLanguages.SelectedValue;
        }
        set
        {
            // Set plain CSS by default
            mValue = ValidationHelper.GetString(value, CssStylesheetInfo.PLAIN_CSS);
            ListItem selItem = Items.FindByValue(mValue, true);
            if (selItem != null)
            {
                ddlLanguages.SelectedValue = selItem.Value;
                lblSelectedLang.Text = selItem.Text;
            }
        }
    }


    /// <summary>
    /// Gets or sets JavaScript code to be executed when 'Change' button is clicked.
    /// </summary>
    public string OnButtonClientClick
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OnClientClick"), String.Empty);
        }
        set
        {
            SetValue("OnClientClick", value);
        }
    }


    /// <summary>
    /// Items of the drop-down list control which contains style sheet language options.
    /// </summary>
    private ListItemCollection Items
    {
        get
        {
            if (ddlLanguages.Items.Count == 0)
            {
                ddlLanguages.Items.Add(new ListItem(GetString("cssstylesheet.plaincss"), CssStylesheetInfo.PLAIN_CSS));

                foreach (CssPreprocessor p in CssStylesheetInfoProvider.CssPreprocessors.Values)
                {
                    ddlLanguages.Items.Add(new ListItem(p.DisplayName, p.Name));
                }
            }

            return ddlLanguages.Items;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnInit control event handling.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;
        }
    }


    /// <summary>
    /// After save form event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Form_OnAfterSave(object sender, EventArgs e)
    {
        plcLanguage.Visible = true;
        plcSelect.Visible = false;
    }


    /// <summary>
    /// OnPreRender control event handling.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Clear script loaded from viewstate and create new one.
        btnChange.OnClientClick = String.Empty;

        // Hide the Change button if the stylesheet object is checked-in or checked out by another user
        CssStylesheetInfo cssInfo = UIContext.EditedObject as CssStylesheetInfo;
        if ((cssInfo != null) && SynchronizationHelper.UseCheckinCheckout)
        {
            btnOpenSelection.Visible = cssInfo.Generalized.IsCheckedOut && cssInfo.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser);
        }

        // Hide the change button if the user does not have permissions to modify CSS stylesheets
        btnOpenSelection.Visible = CurrentUser.IsAuthorizedPerResource("CMS.Design", "ModifyCMSCSSStylesheet");

        // Show confirmation if current language is not plain CSS
        if ((cssInfo != null) && !cssInfo.StylesheetDynamicLanguage.EqualsCSafe(CssStylesheetInfo.PLAIN_CSS, true))
        {
            string index = Data["StylesheetDynamicLanguage"] as string;
            if (!String.IsNullOrEmpty(index))
            {
                index = Items.IndexOf(Items.FindByValue(index)).ToString();
            }
            string script = "var el = document.getElementById('" + ddlLanguages.ClientID + "'); if (el && (el.selectedIndex != " + index + ")) { if (!confirm(" + ScriptHelper.GetString(GetString("cssstylesheet.languagechangeconfirmation")) + ")) return false;  }";
            btnChange.OnClientClick = script;
        }

        if (!String.IsNullOrEmpty(OnButtonClientClick))
        {
            btnChange.OnClientClick += OnButtonClientClick;
        }
    }


    /// <summary>
    /// Page 'Load' event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set up resource strings for labels
        btnChange.ResourceString = "cssstylesheet.changelanguage";
        btnOpenSelection.ResourceString = "cssstylesheet.selectlanguage";

        // Click event handlers
        btnChange.Click += btnChange_Click;
        btnOpenSelection.Click += btnOpenSelection_Click;

        if (!RequestHelper.IsPostBack())
        {
            SetupControls();

            if (Form != null)
            {
                if (Form.IsInsertMode)
                {
                    // Show only drop-down list in insert mode
                    btnChange.Visible = false;
                    plcLanguage.Visible = false;
                    plcSelect.Visible = true;
                }
                else
                {
                    // Show label with button in edit mode
                    plcLanguage.Visible = true;
                    plcSelect.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Setup nested control.
    /// </summary>
    private void SetupControls()
    {
        // Set label text to currently selected language name.
        ListItem selItem = Items.FindByValue(mValue, true);

        if ((selItem == null) || String.IsNullOrWhiteSpace(selItem.Text))
        {
            lblSelectedLang.Visible = false;
        }
        else
        {
            lblSelectedLang.Text = selItem.Text;
        }
    }


    /// <summary>
    /// Click event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void btnOpenSelection_Click(object sender, EventArgs e)
    {
        plcLanguage.Visible = false;
        plcSelect.Visible = true;
    }


    /// <summary>
    /// Click event handling.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void btnChange_Click(object sender, EventArgs e)
    {
        // In this phase the post data are already available. Store the selected value in the variable.
        mValue = ddlLanguages.SelectedValue;
        SetupControls();
        plcLanguage.Visible = true;
        plcSelect.Visible = false;
        RaiseOnChanged();
    }

    #endregion
}