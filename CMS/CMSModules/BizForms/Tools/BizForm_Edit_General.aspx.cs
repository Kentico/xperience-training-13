using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("CMS.Form", "Forms.General")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_General : CMSBizFormPage
{
    #region "Variables"

    protected BizFormInfo bfi;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        rfvDisplayName.Text = GetString("BizFormGeneral.rfvDisplayName");
        lblButtonText.AssociatedControlClientID = txtButtonText.TextBox.ClientID;

        bfi = EditedObject as BizFormInfo;

        if ((!RequestHelper.IsPostBack()) && (bfi != null))
        {
            LoadData();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load data to fields.
    /// </summary>
    protected void LoadData()
    {
        txtDisplayName.Text = bfi.FormDisplayName;
        txtCodeName.Text = bfi.FormName;

        DataClassInfo mDci = DataClassInfoProvider.GetDataClassInfo(bfi.FormClassID);
        txtTableName.Text = mDci.ClassTableName;
        txtClassName.Text = mDci.ClassName;

        txtButtonText.Text = bfi.FormSubmitButtonText;
        txtButtonText.TextBox.WatermarkText = GetString("general.submit");
        txtSubmitButtonImage.Text = bfi.FormSubmitButtonImage;

        txtDisplay.Enabled = false;
        txtRedirect.Enabled = false;

        // Initialize 'after submitting' behavior
        if (!string.IsNullOrEmpty(bfi.FormDisplayText))
        {
            txtDisplay.Text = bfi.FormDisplayText;
            radDisplay.Checked = true;
            txtDisplay.Enabled = true;
        }
        else
        {
            if (!string.IsNullOrEmpty(bfi.FormRedirectToUrl))
            {
                txtRedirect.Text = bfi.FormRedirectToUrl;
                radRedirect.Checked = true;
                txtRedirect.Enabled = true;
            }
            else
            {
                if (bfi.FormClearAfterSave)
                {
                    radClear.Checked = true;
                }
                else
                {
                    radContinue.Checked = true;
                }
            }
        }

        radContinue.Visible = false;
    }


    /// <summary>
    /// Save data to Database.
    /// </summary>
    protected void SaveData()
    {
        // Check display name emptiness
        if (string.IsNullOrEmpty(txtDisplayName.Text))
        {
            ShowError(GetString("BizFormGeneral.rfvDisplayName"));
            return;
        }

        bfi.FormDisplayName = txtDisplayName.Text;
        bfi.FormName = txtCodeName.Text;
        bfi.FormSubmitButtonText = txtButtonText.Text;

        bfi.FormSubmitButtonImage = null;
        if (!string.IsNullOrEmpty(txtSubmitButtonImage.Text.Trim()))
        {
            bfi.FormSubmitButtonImage = txtSubmitButtonImage.Text.Trim();
        }

        // Set 'after submitting' behavior...
        bfi.FormRedirectToUrl = null;
        bfi.FormDisplayText = null;

        // ... clear form
        bfi.FormClearAfterSave = radClear.Checked;

        // ... display text
        if (radDisplay.Checked)
        {
            if (!string.IsNullOrEmpty(txtDisplay.Text.Trim()))
            {
                bfi.FormDisplayText = txtDisplay.Text.Trim();
            }
            else
            {
                ShowError(GetString("BizFormGeneral.DisplayText"));
                return;
            }
        }
        else
        {
            txtDisplay.Text = string.Empty;
        }

        // ... redirect
        if (radRedirect.Checked)
        {
            bfi.FormRedirectToUrl = txtRedirect.Text.Trim();
        }
        else
        {
            txtRedirect.Text = string.Empty;
        }

        BizFormInfo.Provider.Set(bfi);

        ShowChangesSaved();

        // Reload header if changes were saved
        ScriptHelper.RefreshTabHeader(Page, bfi.FormDisplayName);
    }

    #endregion


    #region "Event handlers"

    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }

        SaveData();
    }


    protected void radDisplay_CheckedChanged(object sender, EventArgs e)
    {
        txtDisplay.Enabled = false;
        txtRedirect.Enabled = false;
        if (radDisplay.Checked)
        {
            txtDisplay.Enabled = true;
        }
    }


    protected void radRedirect_CheckedChanged(object sender, EventArgs e)
    {
        txtDisplay.Enabled = false;
        txtRedirect.Enabled = false;
        if (radRedirect.Checked)
        {
            txtRedirect.Enabled = true;
        }
    }


    protected void radClear_CheckedChanged(object sender, EventArgs e)
    {
        txtDisplay.Enabled = false;
        txtRedirect.Enabled = false;
    }

    #endregion
}
