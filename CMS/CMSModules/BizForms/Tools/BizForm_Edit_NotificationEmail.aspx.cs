using System;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("CMS.Form", "Forms.NotificationEmail")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_NotificationEmail : CMSBizFormPage
{
    #region "Variables"

    private DataClassInfo formClassObj;
    protected SaveAction save = null;
    protected BizFormInfo formInfo = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether custom form layout is set or not.
    /// </summary>
    private bool IsLayoutSet
    {
        get
        {
            object obj = ViewState["IsLayoutSet"];
            return (obj == null) ? false : (bool)obj;
        }
        set
        {
            ViewState["IsLayoutSet"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Add save action
        save = new SaveAction();
        menu.AddAction(save);

        // Register for action
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, lnkSave_Click);
    
        // Control initialization
        ltlConfirmDelete.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("Bizform_Edit_Notificationemail.ConfirmDelete") + "\">";

        chkSendToEmail.Text = GetString("BizFormGeneral.chkSendToEmail");

        chkCustomLayout.Text = GetString("BizForm_Edit_NotificationEmail.CustomLayout");

        // Initialize HTML editor
        InitHTMLEditor();

        formInfo = EditedObject as BizFormInfo;

        if (!RequestHelper.IsPostBack())
        {
            if (formInfo!= null)
            {
                // Get form class object
                formClassObj = DataClassInfoProvider.GetDataClassInfo(formInfo.FormClassID);

                // Fill list of available fields                    
                FillFieldsList();

                // Load email from/to address and email subject
                txtFromEmail.Text = ValidationHelper.GetString(formInfo.FormSendFromEmail, "");
                txtToEmail.Text = ValidationHelper.GetString(formInfo.FormSendToEmail, "");
                txtSubject.Text = ValidationHelper.GetString(formInfo.FormEmailSubject, "");
                chkAttachDocs.Checked = formInfo.FormEmailAttachUploadedDocs;
                chkSendToEmail.Checked = ((txtFromEmail.Text + txtToEmail.Text) != "");
                if (!chkSendToEmail.Checked)
                {
                    txtFromEmail.Enabled = false;
                    txtToEmail.Enabled = false;
                    txtSubject.Enabled = false;
                    chkAttachDocs.Enabled = false;
                    chkCustomLayout.Visible = false;
                    pnlCustomLayout.Visible = false;
                }
                else
                {
                    // Enable or disable form
                    EnableDisableForm(formInfo.FormEmailTemplate);
                }
            }
            else
            {
                // Disable form by default
                EnableDisableForm(null);
            }
        }
    }


    protected void Page_PreRender(Object sender, EventArgs e)
    {
        if (pnlCustomLayout.Visible)
        {
            btnInsertInput.OnClientClick = "InsertAtCursorPosition('$$value:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
            btnInsertLabel.OnClientClick = "InsertAtCursorPosition('$$label:' + document.getElementById('" + lstAvailableFields.ClientID + "').value + '$$'); return false;";
        }

        // Get save script
        string script = null;
        if (!pnlCustomLayout.Visible && IsLayoutSet)
        {
            script = "if(!ConfirmDelete()) { return false; } ";
        }

        // Refresh script
        save.OnClientClick = script;
        menu.ReloadData();

        // Register other scripts
        RegisterScripts();
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// On chkSendToEmail checked event handler.
    /// </summary>
    protected void chkSendToEmail_CheckedChanged(object sender, EventArgs e)
    {
        txtFromEmail.Enabled = chkSendToEmail.Checked;
        txtToEmail.Enabled = chkSendToEmail.Checked;
        txtSubject.Enabled = chkSendToEmail.Checked;
        chkAttachDocs.Enabled = chkSendToEmail.Checked;
        if (chkSendToEmail.Checked)
        {
            chkCustomLayout.Visible = true;
            if (chkCustomLayout.Checked)
            {
                pnlCustomLayout.Visible = true;

                // Reload HTML editor content
                if (formInfo != null && formInfo.FormEmailTemplate != null)
                {
                    htmlEditor.ResolvedValue = formInfo.FormEmailTemplate;
                }
            }
        }
        else
        {
            chkCustomLayout.Visible = false;
            pnlCustomLayout.Visible = false;
        }
    }


    /// <summary>
    /// Custom layout checkbox checked changed.
    /// </summary>
    protected void chkCustomLayout_CheckedChanged(object sender, EventArgs e)
    {
        pnlCustomLayout.Visible = !pnlCustomLayout.Visible;

        if (chkCustomLayout.Checked)
        {
            if (formInfo != null && formInfo.FormEmailTemplate != null)
            {
                htmlEditor.ResolvedValue = formInfo.FormEmailTemplate;
            }
        }
    }


    /// <summary>
    /// Save button is clicked.
    /// </summary>
    protected void lnkSave_Click(object sender, EventArgs e)
    {
        // Check 'EditForm' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
        {
            RedirectToAccessDenied("cms.form", "EditForm");
        }

        string errorMessage = null;

        if (formInfo != null)
        {
            if (chkSendToEmail.Checked)
            {
                // Validate form
                errorMessage = new Validator().NotEmpty(txtFromEmail.Text, GetString("BizFormGeneral.EmptyFromEmail"))
                    .NotEmpty(txtToEmail.Text, GetString("BizFormGeneral.EmptyToEmail"))
                    .NotEmpty(txtSubject.Text, GetString("BizFormGeneral.EmptyEmailSubject")).Result;

                // Check if to e-mail contains macro expression or e-mails separated by semicolon
                if (string.IsNullOrEmpty(errorMessage) && !MacroProcessor.ContainsMacro(txtToEmail.Text.Trim()) && !txtToEmail.IsValid())
                {
                    errorMessage = GetString("BizFormGeneral.EmptyToEmail");
                }

                // Check if from e-mail contains macro expression or e-mails separated by semicolon
                if (string.IsNullOrEmpty(errorMessage) && !MacroProcessor.ContainsMacro(txtFromEmail.Text.Trim()) && !txtFromEmail.IsValid())
                {
                    errorMessage = GetString("BizFormGeneral.EmptyFromEmail");
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    formInfo.FormSendFromEmail = txtFromEmail.Text.Trim();
                    formInfo.FormSendToEmail = txtToEmail.Text.Trim();
                    formInfo.FormEmailSubject = txtSubject.Text.Trim();
                    formInfo.FormEmailAttachUploadedDocs = chkAttachDocs.Checked;
                    if (chkCustomLayout.Checked)
                    {
                        formInfo.FormEmailTemplate = htmlEditor.ResolvedValue.Trim();
                    }
                    else
                    {
                        formInfo.FormEmailTemplate = null;
                    }
                }
            }
            else
            {
                formInfo.FormSendFromEmail = null;
                formInfo.FormSendToEmail = null;
                formInfo.FormEmailSubject = null;
                formInfo.FormEmailTemplate = null;
                txtToEmail.Text = string.Empty;
                txtFromEmail.Text = string.Empty;
                txtSubject.Text = string.Empty;
                chkAttachDocs.Checked = true;
                htmlEditor.ResolvedValue = string.Empty;
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                try
                {
                    BizFormInfoProvider.SetBizFormInfo(formInfo);
                    ShowChangesSaved();
                    EnableDisableForm(formInfo.FormEmailTemplate);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Fills list of available fields.
    /// </summary>
    private void FillFieldsList()
    {
        if (formClassObj != null)
        {
            // Load form definition and get visible fields
            var fi = FormHelper.GetFormInfo(formClassObj.ClassName, false);
            var fields = fi.GetFields(true, true);

            lstAvailableFields.Items.Clear();

            if (fields != null)
            {
                // Add visible fields to the list
                foreach (FormFieldInfo ffi in fields)
                {
                    lstAvailableFields.Items.Add(new ListItem(ffi.GetDisplayName(MacroResolver.GetInstance()), ffi.Name));
                }
            }
            lstAvailableFields.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Enables or disables form according to form layout is defined or not.
    /// </summary>
    protected void EnableDisableForm(string formLayout)
    {
        // if form layout is set
        if (formLayout != null)
        {
            //enable form editing                    
            chkCustomLayout.Checked = true;
            pnlCustomLayout.Visible = true;

            // set text (form layout) to the editable window of the HTML editor
            htmlEditor.ResolvedValue = formLayout;

            // save info to viewstate 
            IsLayoutSet = true;
        }
        else
        {
            // form is not enabled by default        
            chkCustomLayout.Checked = false;
            pnlCustomLayout.Visible = false;

            htmlEditor.Value = string.Empty;

            // save info to viewstate
            IsLayoutSet = false;
        }
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlEditor.AutoDetectLanguage = false;
        htmlEditor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEditor.ToolbarSet = "BizForm";
        htmlEditor.MediaDialogConfig.UseFullURL = true;
        htmlEditor.LinkDialogConfig.UseFullURL = true;
        htmlEditor.QuickInsertConfig.UseFullURL = true;
    }


    /// <summary>
    /// Registers JS scripts.
    /// </summary>
    protected void RegisterScripts()
    {
        string script =
string.Format(@"// Insert desired HTML at the current cursor position of the CK editor
function InsertHTML(htmlString) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Check the active editing mode.
    if (oEditor.mode == 'wysiwyg') {{
        // Insert the desired HTML.
        oEditor.insertHtml(htmlString);
    }}
    else
        alert('You must be on WYSIWYG mode!');
}}

// Set content of the CK editor - replace the actual one
function SetContent(newContent) {{
    // Get the editor instance that we want to interact with.
    var oEditor = CKEDITOR.instances['{0}'];

    // Set the editor content (replace the actual one).
    oEditor.setData(newContent);
}}

// Returns HTML code with standard table layout
function GenerateTableLayout() {{
    var tableLayout = """";

    // indicates whether any row definition was added to the table
    var rowAdded = false;

    // list of attributes
    var list = document.getElementById('{1}');

    // attributes count
    var optionsCount = 0;
    if (list != null) {{
        optionsCount = list.options.length;
    }}

    for (var i = 0; i < optionsCount; i++) {{
        tableLayout += ""<tr><td>$$label:"" + list.options[i].value + ""$$</td><td>$$value:"" + list.options[i].value + ""$$</td></tr>"";
        rowAdded = true;
    }}

    if (rowAdded) {{
        tableLayout = ""<table><tbody>"" + tableLayout + ""</tbody></table>"";
    }}

    return tableLayout;
}}

// Insert desired HTML at the current cursor position of the CK editor if it is not already inserted 
function InsertAtCursorPosition(htmlString) {{
    InsertHTML(htmlString);
}}

function ConfirmDelete() {{
    return confirm(document.getElementById('confirmdelete').value);
}}", htmlEditor.ClientID, lstAvailableFields.ClientID);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "NotificationEmail_" + ClientID, script, true);
    }

    #endregion
}
